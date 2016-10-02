using Microsoft.Xna.Framework;
using System;

namespace GameBoyMono
{
    public partial class GameBoyCPU
    {
        public GeneralMemory generalMemory = new GeneralMemory();
        public Cartridge cartridge = new Cartridge();

        // Interrupt Master Enable Flag (write only)
        public bool IME;
        
        // registers
        public byte reg_A, reg_F;
        public byte reg_B, reg_C;
        public byte reg_D, reg_E;
        public byte reg_H, reg_L;

        public ushort reg_AF { get { return (ushort)(reg_A << 8 | reg_F); } set { reg_A = (byte)(value >> 8); reg_F = (byte)(value & 0xF0); } }
        public ushort reg_BC { get { return (ushort)(reg_B << 8 | reg_C); } set { reg_B = (byte)(value >> 8); reg_C = (byte)(value & 0xFF); } }
        public ushort reg_DE { get { return (ushort)(reg_D << 8 | reg_E); } set { reg_D = (byte)(value >> 8); reg_E = (byte)(value & 0xFF); } }
        public ushort reg_HL { get { return (ushort)(reg_H << 8 | reg_L); } set { reg_H = (byte)(value >> 8); reg_L = (byte)(value & 0xFF); } }
        
        // flags (reg_F)
        public bool flag_Z { get { return (reg_F & 0x80) == 0x80; } set { reg_F = (byte)((reg_F & 0x70) | (value ? 0x80 : 0x00)); } }
        public bool flag_N { get { return (reg_F & 0x40) == 0x40; } set { reg_F = (byte)((reg_F & 0xB0) | (value ? 0x40 : 0x00)); } }
        public bool flag_H { get { return (reg_F & 0x20) == 0x20; } set { reg_F = (byte)((reg_F & 0xD0) | (value ? 0x20 : 0x00)); } }
        public bool flag_C { get { return (reg_F & 0x10) == 0x10; } set { reg_F = (byte)((reg_F & 0xE0) | (value ? 0x10 : 0x00)); } }

        byte flag_CBit { get { return (byte)(flag_C ? 1 : 0); } }

        // SP - stack point
        public ushort reg_SP;
        // PC - program counter
        public ushort reg_PC;

        public byte data8 { get { return generalMemory[reg_PC - 1]; } }
        public ushort data16 { get { return (ushort)((generalMemory[reg_PC - 1] << 8) | generalMemory[reg_PC - 2]); } }

        bool CPUHalt;
        
        // list of functions
        public Action[] ops, op_cb;

        //  4194304Hz / 59.73fps = 70221
        public int maxCycles = 70224, cycleCount, lastCycleCount;  // 69905 70224
        int lcdCycleCount, lcdCycleTime;

        // 44100/60=735 -> 70224/735=95,5..
        float soundCount, maxSoundCycles = 95.54f;

        public bool romMounted, renderScreen;

        public bool soundPlaying;

        int divCounter, timerCounter;

        int soundDelay = 0;

        public byte LY { get { return generalMemory[0xFF44]; } }
        public byte LYC { get { return generalMemory.memory[0xFF45]; } }
        public byte LCDMode { get { return (byte)(generalMemory.memory[0xFF41] & 0x03); } }

        public byte Stat { get { return generalMemory.memory[0xFF41]; } }
        
        public GameBoyCPU()
        {
            ops = new Action[] {
                NOP, LD_BC_d16, LD_aBC_A, INC_BC, INC_B, DEC_B, LD_B_d8, RLCA, LD_a16_SP, ADD_HL_BC, LD_A_aBC, DEC_BC, INC_C, DEC_C, LD_C_d8, RRCA,
                STOP, LD_DE_d16, LD_aDE_A, INC_DE, INC_D, DEC_D, LD_D_d8, RLA, JR_d8, ADD_HL_DE, LD_A_aDE, DEC_DE, INC_E, DEC_E, LD_E_d8, RRA,
                JR_NZ_a8, LD_HL_d16, LD_aHLp_A, INC_HL, INC_H, DEC_H, LD_H_d8, DAA, JR_Z_a8, ADD_HL_HL, LD_A_aHLp, DEC_HL, INC_L, DEC_L, LD_L_d8, CPL,
                JR_NC_a8, LD_SP_d16, LD_aHLm_A, INC_SP, INC_aHL, DEC_aHL, LD_aHL_d8, SCF, JR_C_a8, ADD_HL_SP, LD_A_aHLm, DEC_SP, INC_A, DEC_A, LD_A_d8, CCF,
                LD_B_B, LD_B_C, LD_B_D, LD_B_E, LD_B_H, LD_B_L, LD_B_aHL, LD_B_A, LD_C_B, LD_C_C, LD_C_D, LD_C_E, LD_C_H, LD_C_L, LD_C_aHL, LD_C_A,
                LD_D_B, LD_D_C, LD_D_D, LD_D_E, LD_D_H, LD_D_L, LD_D_aHL, LD_D_A, LD_E_B, LD_E_C, LD_E_D, LD_E_E, LD_E_H, LD_E_L, LD_E_aHL, LD_E_A,
                LD_H_B, LD_H_C, LD_H_D, LD_H_E, LD_H_H, LD_H_L, LD_H_aHL, LD_H_A, LD_L_B, LD_L_C, LD_L_D, LD_L_E, LD_L_H, LD_L_L, LD_L_aHL, LD_L_A,
                LD_aHL_B, LD_aHL_C, LD_aHL_D, LD_aHL_E, LD_aHL_H, LD_aHL_L, HALT, LD_aHL_A, LD_A_B, LD_A_C, LD_A_D, LD_A_E, LD_A_H, LD_A_L, LD_A_aHL, LD_A_A,
                ADD_A_B, ADD_A_C, ADD_A_D, ADD_A_E, ADD_A_H, ADD_A_L, ADD_A_aHL, ADD_A_A, ADC_A_B, ADC_A_C, ADC_A_D, ADC_A_E, ADC_A_H, ADC_A_L, ADC_A_aHL, ADC_A_A,
                SUB_B, SUB_C, SUB_D, SUB_E, SUB_H, SUB_L, SUB_aHL, SUB_A, SBC_A_B, SBC_A_C, SBC_A_D, SBC_A_E, SBC_A_H, SBC_A_L, SBC_A_aHL, SBC_A_A,
                AND_B, AND_C, AND_D, AND_E, AND_H, AND_L, AND_aHL, AND_A, XOR_B, XOR_C, XOR_D, XOR_E, XOR_H, XOR_L, XOR_aHL, XOR_A,
                OR_B, OR_C, OR_D, OR_E, OR_H, OR_L, OR_aHL, OR_A, CP_B, CP_C, CP_D, CP_E, CP_H, CP_L, CP_aHL, CP_A,
                RET_NZ, POP_BC, JP_NZ_a16, JP_a16, CALL_NZ_a16, PUSH_BC, ADD_A_d8, RST_00H, RET_Z, RET, JP_Z_a16, null, CALL_Z_a16, CALL_a16, ADC_A_d8, RST_08H,
                RET_NC, POP_DE, JP_NC_a16, null, CALL_NC_a16, PUSH_DE, SUB_d8, RST_10H, RET_C, RETI, JP_C_a16, null, CALL_C_a16, null, SBC_A_d8, RST_18H,
                LDH_a8_A, POP_HL, LD_aC_A, null, null, PUSH_HL, AND_d8, RST_20H, ADD_SP_r8, JP_aHL, LD_a16_A, null, null, null, XOR_d8, RST_28H,
                LDH_A_a8, POP_AF, LD_A_aC, DI, null, PUSH_AF, OR_d8, RST_30H, LD_HL_SPr8, LD_SP_HL, LD_A_a16, EI, null, null, CP_d8, RST_38H};

            op_cb = new Action[] {
                RLC_B, RLC_C, RLC_D, RLC_E, RLC_H, RLC_L, RLC_aHL, RLC_A, RRC_B, RRC_C, RRC_D, RRC_E, RRC_H, RRC_L, RRC_aHL, RRC_A,
                RL_B, RL_C, RL_D, RL_E, RL_H, RL_L, RL_aHL, RL_A, RR_B, RR_C, RR_D, RR_E, RR_H, RR_L, RR_aHL, RR_A ,
                SLA_B, SLA_C, SLA_D, SLA_E, SLA_H, SLA_L, SLA_aHL, SLA_A, SRA_B, SRA_C, SRA_D, SRA_E, SRA_H, SRA_L, SRA_aHL, SRA_A,
                SWAP_B, SWAP_C, SWAP_D, SWAP_E, SWAP_H, SWAP_L, SWAP_aHL, SWAP_A, SRL_B, SRL_C, SRL_D, SRL_E, SRL_H, SRL_L, SRL_aHL, SRL_A,
                BIT_0_B, BIT_0_C, BIT_0_D, BIT_0_E, BIT_0_H, BIT_0_L, BIT_0_aHL, BIT_0_A, BIT_1_B, BIT_1_C, BIT_1_D, BIT_1_E, BIT_1_H, BIT_1_L, BIT_1_aHL, BIT_1_A,
                BIT_2_B, BIT_2_C, BIT_2_D, BIT_2_E, BIT_2_H, BIT_2_L, BIT_2_aHL, BIT_2_A, BIT_3_B, BIT_3_C, BIT_3_D, BIT_3_E, BIT_3_H, BIT_3_L, BIT_3_aHL, BIT_3_A,
                BIT_4_B, BIT_4_C, BIT_4_D, BIT_4_E, BIT_4_H, BIT_4_L, BIT_4_aHL, BIT_4_A, BIT_5_B, BIT_5_C, BIT_5_D, BIT_5_E, BIT_5_H, BIT_5_L, BIT_5_aHL, BIT_5_A,
                BIT_6_B, BIT_6_C, BIT_6_D, BIT_6_E, BIT_6_H, BIT_6_L, BIT_6_aHL, BIT_6_A, BIT_7_B, BIT_7_C, BIT_7_D, BIT_7_E, BIT_7_H, BIT_7_L, BIT_7_aHL, BIT_7_A,
                RES_0_B, RES_0_C, RES_0_D, RES_0_E, RES_0_H, RES_0_L, RES_0_aHL, RES_0_A, RES_1_B, RES_1_C, RES_1_D, RES_1_E, RES_1_H, RES_1_L, RES_1_aHL, RES_1_A,
                RES_2_B, RES_2_C, RES_2_D, RES_2_E, RES_2_H, RES_2_L, RES_2_aHL, RES_2_A, RES_3_B, RES_3_C, RES_3_D, RES_3_E, RES_3_H, RES_3_L, RES_3_aHL, RES_3_A,
                RES_4_B, RES_4_C, RES_4_D, RES_4_E, RES_4_H, RES_4_L, RES_4_aHL, RES_4_A, RES_5_B, RES_5_C, RES_5_D, RES_5_E, RES_5_H, RES_5_L, RES_5_aHL, RES_5_A,
                RES_6_B, RES_6_C, RES_6_D, RES_6_E, RES_6_H, RES_6_L, RES_6_aHL, RES_6_A, RES_7_B, RES_7_C, RES_7_D, RES_7_E, RES_7_H, RES_7_L, RES_7_aHL, RES_7_A,
                SET_0_B, SET_0_C, SET_0_D, SET_0_E, SET_0_H, SET_0_L, SET_0_aHL, SET_0_A, SET_1_B, SET_1_C, SET_1_D, SET_1_E, SET_1_H, SET_1_L, SET_1_aHL, SET_1_A,
                SET_2_B, SET_2_C, SET_2_D, SET_2_E, SET_2_H, SET_2_L, SET_2_aHL, SET_2_A, SET_3_B, SET_3_C, SET_3_D, SET_3_E, SET_3_H, SET_3_L, SET_3_aHL, SET_3_A,
                SET_4_B, SET_4_C, SET_4_D, SET_4_E, SET_4_H, SET_4_L, SET_4_aHL, SET_4_A, SET_5_B, SET_5_C, SET_5_D, SET_5_E, SET_5_H, SET_5_L, SET_5_aHL, SET_5_A,
                SET_6_B, SET_6_C, SET_6_D, SET_6_E, SET_6_H, SET_6_L, SET_6_aHL, SET_6_A, SET_7_B, SET_7_C, SET_7_D, SET_7_E, SET_7_H, SET_7_L, SET_7_aHL, SET_7_A };
        }

        public void Start()
        {
            generalMemory.Init();

            cycleCount = 0;
            lastCycleCount = 0;
            lcdCycleCount = 0;
            lcdCycleTime = 0;

            CPUHalt = false;

            SkipBootROM();
        }
        
        public void Update(GameTime gametime)
        {
            while (cycleCount < maxCycles)
                CPUCycle();

            cycleCount -= maxCycles;

            // render Screen
            //if (renderScreen)
            //{
            //    renderScreen = false;
            //    Game1.gbRenderer.Draw();
            //}

            Game1.gbSound.AddCurrentBuffer();

            if (!soundPlaying)
            {
                soundDelay++;

                if (soundDelay >= 4)
                {
                    soundPlaying = true;
                    Game1.gbSound.Play();
                }
            }
        }

        public void CPUCycle()
        {
            lastCycleCount = cycleCount;

            // execute next instruction
            if (!CPUHalt)
                executeInstruction();
            else
                cycleCount += 4;

            soundCount += (cycleCount - lastCycleCount);

            if (soundCount > maxSoundCycles)
            {
                soundCount -= maxSoundCycles;

                Game1.gbSound.UpdateBuffer();
            }

            int oldStat = generalMemory.memory[0xFF41];

            // display enabled?
            if ((Game1.gbCPU.generalMemory.memory[0xFF40] & 0x80) == 0x80)
            {
                lcdCycleCount += (cycleCount - lastCycleCount);

                // http://gameboy.mongenel.com/dmg/istat98.txt

                if (lcdCycleCount > lcdCycleTime)
                {
                    lcdCycleCount -= lcdCycleTime;

                    // set the LY byte
                    generalMemory.memory[0xFF44]++;
                    // reste LCD
                    if (generalMemory.memory[0xFF44] > 153)
                        generalMemory.memory[0xFF44] = 0;

                    lcdCycleTime = 456;
                    if (generalMemory[0xFF44] == 0x98)
                        lcdCycleTime = 56;
                    else if (generalMemory[0xFF44] == 0x99)
                        lcdCycleTime = 856;

                    // render scanline
                    if (generalMemory.memory[0xFF44] < 144)
                        Game1.gbRenderer.RenderLine(generalMemory.memory[0xFF44]);

                    // v-blank flag
                    if (generalMemory.memory[0xFF44] == 144)
                    {
                        renderScreen = true;

                        // set IF flag
                        generalMemory.memory[0xFF0F] |= 0x01;
                        // disable HALT
                        //CPUHalt = false;
                    }
                }

                // set mode flag
                // mode 2,03,0 cycle
                // cycle: 456 clks * 144
                // 2: 77-84     (80)    read oam memory
                // 3: 169-175   (172)   transf data to lcd
                // 0: 201-207   (204)   H-Blank 
                // LY=153: 56cl     LY=0: 856
                // 1:                   V-Blank
                // 144*456(65.664) + 4560 = 70224
                if (generalMemory.memory[0xFF44] >= 144)
                    generalMemory.memory[0xFF41] = (byte)((generalMemory.memory[0xFF41] & 0xFC) | 0x01);
                else if (lcdCycleCount < 80)
                    generalMemory.memory[0xFF41] = (byte)((generalMemory.memory[0xFF41] & 0xFC) | 0x02);
                else if (lcdCycleCount < 252)
                    generalMemory.memory[0xFF41] = (byte)((generalMemory.memory[0xFF41] & 0xFC) | 0x03);
                else
                    generalMemory.memory[0xFF41] = (byte)((generalMemory.memory[0xFF41] & 0xFC) | 0x00);
            }
            else
            {
                generalMemory.memory[0xFF41] &= 0xFC;
                //generalMemory.memory[0xFF41] |= 0x01;
                //generalMemory.memory[0xFF44] = 0;
                //lcdCycleCount = 0;
            }


            // set coincidence flag (0:LYC<>LY, 1:LYC=LY)
            if (generalMemory.memory[0xFF44] == generalMemory.memory[0xFF45])
                generalMemory.memory[0xFF41] |= 0x04;
            else
                generalMemory.memory[0xFF41] &= 0xFB;

            byte STAT = generalMemory.memory[0xFF41];

            if (((generalMemory.memory[0xFF41] & 0x40) == 0x40 && generalMemory.memory[0xFF44] == generalMemory.memory[0xFF45]) ||          // LY == LYC
                ((generalMemory.memory[0xFF41] & 0x20) == 0x20 && (oldStat & 0x03) == 0 && (generalMemory.memory[0xFF41] & 0x03) == 1) ||   // OAM (start of mode 1 and 2)
                ((generalMemory.memory[0xFF41] & 0x20) == 0x20 && (oldStat & 0x03) == 1 && (generalMemory.memory[0xFF41] & 0x03) == 2) ||
                ((generalMemory.memory[0xFF41] & 0x10) == 0x10 && (oldStat & 0x03) == 0 && (generalMemory.memory[0xFF41] & 0x03) == 1) ||   // v-blank
                ((generalMemory.memory[0xFF41] & 0x08) == 0x08 && (oldStat & 0x03) == 3 && (generalMemory.memory[0xFF41] & 0x03) == 0))     // h-blank (start of mode 0)
            {
                // stat interrupt
                generalMemory.memory[0xFF0F] |= 0x02;
                // disable HALT
                //CPUHalt = false;
            }
            //}

            // 16384Hz increment:
            // 16384 / 60 = 273
            // 70224 / 273 = 257
            divCounter += (cycleCount - lastCycleCount);
            if (divCounter >= 256)
            {
                divCounter -= 256;
                generalMemory.memory[0xFF04]++;
            }

            // FF07 - TAC - Timer Control
            // Bit  2     - Timer Stop  (0=Stop, 1=Start)
            // Bits 1 - 0 - Input Clock Select
            // Timer counter update
            if ((generalMemory.memory[0xFF07] & 0x04) == 0x04)
            {
                timerCounter += (cycleCount - lastCycleCount);

                int threshold = 0;
                // 00:   4096 Hz(~4194 Hz SGB)
                // 01: 262144 Hz(~268400 Hz SGB)
                // 10:  65536 Hz(~67110 Hz SGB)
                // 11:  16384 Hz(~16780 Hz SGB
                switch (generalMemory.memory[0xFF07] & 0x03)
                {
                    case 0x00: threshold = 64; break;
                    case 0x01: threshold = 1; break;
                    case 0x02: threshold = 4; break;
                    case 0x03: threshold = 16; break;
                }
                // 4096
                // 262144Hz / 256 = 1024
                // 1024 / 60fps = 17
                if (timerCounter >= 16 * threshold)
                {
                    timerCounter -= 16 * threshold;

                    // timer interrupt
                    if (generalMemory.memory[0xFF05] == 0xFF)
                    {
                        int TMA = generalMemory.memory[0xFF06];
                        // TIMA = TMA
                        generalMemory.memory[0xFF05] = generalMemory.memory[0xFF06];
                        // set the IF register
                        generalMemory.memory[0xFF0F] |= 0x04;

                        //if ((generalMemory._generalMemory[0xFFFF] & 0x04) == 0x04)
                        //{
                        // disable HALT
                        CPUHalt = false;
                        //}
                    }
                    else
                    {
                        generalMemory.memory[0xFF05]++;
                    }
                }
            }
            else
            {
                generalMemory.memory[0xFF05] = 0;
            }

            // v-blank interrupt
            if (IME && (generalMemory.memory[0xFFFF] & 0x01) == 0x01 && (generalMemory.memory[0xFF0F] & 0x01) == 0x01)
                VblankInterrupt();
            // LCD STAT interrupt
            else if (IME && (generalMemory.memory[0xFFFF] & 0x02) == 0x02 && (generalMemory.memory[0xFF0F] & 0x02) == 0x02)
                StatInterrupt();
            // timer overflow interrupt
            else if (IME && (generalMemory.memory[0xFFFF] & 0x04) == 0x04 && (generalMemory.memory[0xFF0F] & 0x04) == 0x04)
                TimerInterrupt();
        }

        int currentInstruction;
        public void executeInstruction()
        {
            currentInstruction = reg_PC;

            // update cycle count
            cycleCount += OPCycle.cycleArray[generalMemory[reg_PC]];
            reg_PC += OPLenght.opLength[generalMemory[reg_PC]];

            // execute
            if (ops[generalMemory[currentInstruction]] != null)
            {
                ops[generalMemory[currentInstruction]]();
            }
            // cb instruction
            else if (generalMemory[currentInstruction] == 0xCB)
            {
                cycleCount += OPCycle.cycleCBArray[generalMemory[reg_PC]];
                op_cb[generalMemory[reg_PC]]();
                reg_PC++;
            }
            else
            {
                reg_PC++;
            }
        }

        /*
          FFFF - IE - Interrupt Enable (R/W)
          Bit 0: V-Blank  Interrupt Enable  (INT 40h)  (1=Enable)
          Bit 1: LCD STAT Interrupt Enable  (INT 48h)  (1=Enable)
          Bit 2: Timer    Interrupt Enable  (INT 50h)  (1=Enable)
          Bit 3: Serial   Interrupt Enable  (INT 58h)  (1=Enable)
          Bit 4: Joypad   Interrupt Enable  (INT 60h)  (1=Enable)
        */

        public void ExecuteInterrupt()
        {
            // disable interrupts
            IME = false;
            // put the PC 
            generalMemory[--reg_SP] = (byte)(reg_PC >> 8);
            generalMemory[--reg_SP] = (byte)(reg_PC & 0xFF);

            CPUHalt = false;
        }

        /*
         if interrupts are disabled and a ‘halt‘ instruction is executed, 
         then halt doesn’t suspend operation but it does cause the PC to
         stop counting for one instruction.  This is an odd behaviour, 
         but one we need to take into account for an accurate emulation. 
         */

        public void VblankInterrupt()
        {
            ExecuteInterrupt();

            // disable interrupt flag
            generalMemory.memory[0xFF0F] &= 0xFE;
            // jump to address
            reg_PC = 0x40;
        }

        public void StatInterrupt()
        {
            ExecuteInterrupt();

            // disable interrupt flag
            generalMemory.memory[0xFF0F] &= 0xFD;
            // jump to address
            reg_PC = 0x48;
        }

        public void TimerInterrupt()
        {
            ExecuteInterrupt();

            // disable interrupt flag
            generalMemory.memory[0xFF0F] &= 0xFB;
            // jump to address
            reg_PC = 0x50;
        }

        public void SkipBootROM()
        {
            romMounted = true;

            reg_AF = 0x01B0;
            reg_BC = 0x0013;
            reg_DE = 0x00D8;
            reg_HL = 0x014D;

            reg_PC = 0x0100;
            reg_SP = 0xFFFE;

            generalMemory.memory[0xFF06] = 0x00;
            generalMemory.memory[0xFF07] = 0x00;
            generalMemory.memory[0xFF10] = 0x80;
            generalMemory.memory[0xFF11] = 0xBF;
            generalMemory.memory[0xFF12] = 0xF3;
            generalMemory.memory[0xFF14] = 0xBF;
            generalMemory.memory[0xFF16] = 0x3F;
            generalMemory.memory[0xFF17] = 0x00;
            generalMemory.memory[0xFF19] = 0xBF;
            generalMemory.memory[0xFF1A] = 0x7F;
            generalMemory.memory[0xFF1B] = 0xFF;
            generalMemory.memory[0xFF1C] = 0x9F;
            generalMemory.memory[0xFF1E] = 0xBF;
            generalMemory.memory[0xFF20] = 0xFF;
            generalMemory.memory[0xFF21] = 0x00;
            generalMemory.memory[0xFF22] = 0x00;
            generalMemory.memory[0xFF23] = 0xBF;
            generalMemory.memory[0xFF24] = 0x77;
            generalMemory.memory[0xFF25] = 0xF3;
            generalMemory.memory[0xFF26] = 0xF1;
            generalMemory.memory[0xFF40] = 0x91;
            generalMemory.memory[0xFF42] = 0x00;
            generalMemory.memory[0xFF43] = 0x00;
            generalMemory.memory[0xFF45] = 0x00;
            generalMemory.memory[0xFF47] = 0xFC;
            generalMemory.memory[0xFF48] = 0xFF;
            generalMemory.memory[0xFF49] = 0xFF;
            generalMemory.memory[0xFF4A] = 0x00;
            generalMemory.memory[0xFF4B] = 0x00;
            generalMemory.memory[0xFFFF] = 0x00;
        }
    }
}
