using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyMono
{
    public partial class GameBoyCPU
    {
        public byte[] generalMemory = new byte[65536];

        /* 
         * A-accumulator
         * F-flags
         * 
         * 7 6 5 4 3 2 1 0
         * Z N H C 0 0 0 0
         * 
         * Z-Zero Flag
         * N-Subtract Flag
         * H-Half Carry Flag
         * C-Carry Flag
         * 0-Not used, always zero
         */
        public byte reg_A;
        public byte reg_F;
        public byte reg_B;
        public byte reg_C;
        public byte reg_D;
        public byte reg_E;
        public byte reg_H;
        public byte reg_L;

        public ushort reg_AF { get { return (ushort)(reg_A << 8 | reg_F); } set { reg_A = (byte)(value >> 8); reg_F = (byte)(value & 0xFF); } }
        public ushort reg_BC { get { return (ushort)(reg_B << 8 | reg_C); } set { reg_B = (byte)(value >> 8); reg_C = (byte)(value & 0xFF); } }
        public ushort reg_DE { get { return (ushort)(reg_D << 8 | reg_E); } set { reg_D = (byte)(value >> 8); reg_E = (byte)(value & 0xFF); } }
        public ushort reg_HL { get { return (ushort)(reg_H << 8 | reg_L); } set { reg_H = (byte)(value >> 8); reg_L = (byte)(value & 0xFF); } }

        public bool flag_Z { get { return (reg_F >> 7) == 1; } set { reg_F = (byte)((reg_F & 0x7F) | (value ? 0x80 : 0x00)); } }
        public bool flag_N { get { return ((reg_F >> 6) & 0x01) == 1; } set { reg_F = (byte)((reg_F & 0xBF) | (value ? 0x40 : 0x00)); } }
        public bool flag_H { get { return ((reg_F >> 5) & 0x01) == 1; } set { reg_F = (byte)((reg_F & 0xDF) | (value ? 0x20 : 0x00)); } }
        public bool flag_C { get { return ((reg_F >> 4) & 0x01) == 1; } set { reg_F = (byte)((reg_F & 0xEF) | (value ? 0x10 : 0x00)); } }

        /*
         * SP - stack point
         * PC - program counter
         */
        public ushort reg_SP;
        public ushort reg_PC;

        public byte reg_S { get { return (byte)(reg_SP >> 8); } set { reg_SP = (ushort)((value << 8) | (reg_SP & 0x00FF)); } }
        public byte reg_P { get { return (byte)(reg_SP & 0x00FF); } set { reg_SP = (ushort)(value | (reg_SP & 0xFF00)); } }

        public byte data8;
        public ushort data16;

        public byte getGMemory(ushort pos)
        {
            return generalMemory[pos];
        }
        //public void setGMemory(ushort pos, byte bValue)
        //{
        //    generalMemory[pos] = bValue;
        //}

        public void Start()
        {
            // entry point 0100-0103
            reg_PC = 0x100;

            byte b1 = 0xAA;
            b1 = (byte)(~b1);

            Action[] opas = {   NOP, LD_BC_d16, LD_aBC_A, INC_BC, INC_B, DEC_B, LD_B_d8, RLCA, LD_a16_SP, ADD_HL_BC, LD_A_aBC, DEC_BC, INC_C, DEC_C, LD_C_d8, RRCA,
                                STOP, LD_DE_d16, LD_aDE_A, INC_DE, INC_D, DEC_D, LD_D_d8, RLA, JR_d8, ADD_HL_DE, LD_A_aDE, DEC_DE, INC_E, DEC_E, LD_E_d8, RRA,
                                JR_NZ_a8, LD_HL_d16, LD_aHLp_A, INC_HL, INC_H, DEC_H, LD_H_d8, DAA, JR_Z_a8, ADD_HL_HL, LD_A_aHLp, DEC_HL, INC_L, DEC_L, LD_L_d8, CPL,
                                JR_NC_a8, LD_SP_d16, LD_aHLm_A, INC_SP, INC_aHL, DEC_aHL, LD_aHL_d8, SCF, JR_C_a8, ADD_HL_SP, LD_A_aHLm, DEC_SP, INC_A, DEC_A, LD_A_d8, CCF,
                                LD_B_B, LD_B_C, LD_B_D, LD_B_E, LD_B_H, LD_B_L, LD_B_aHL, LD_B_A, LD_C_B, LD_C_C, LD_C_D, LD_C_E, LD_C_H, LD_C_L, LD_C_aHL, LD_C_A,
                                LD_D_B, LD_D_C, LD_D_D, LD_D_E, LD_D_H, LD_D_L, LD_D_aHL, LD_D_A, LD_E_B, LD_E_C, LD_E_D, LD_E_E, LD_E_H, LD_E_L, LD_E_aHL, LD_E_A,
                                LD_H_B, LD_H_C, LD_H_D, LD_H_E, LD_H_H, LD_H_L, LD_H_aHL, LD_H_A, LD_L_B, LD_L_C, LD_L_D, LD_L_E, LD_L_H, LD_L_L, LD_L_aHL, LD_L_A,
                                LD_aHL_B, LD_aHL_C, LD_aHL_D, LD_aHL_E, LD_aHL_H, LD_aHL_L, HALT, LD_aHL_A, LD_A_B, LD_A_C, LD_A_D, LD_A_E, LD_A_H, LD_A_L, LD_A_aHL, LD_A_A,
                                ADD_A_B, ADD_A_C, ADD_A_D, ADD_A_E, ADD_A_H, ADD_A_L, ADD_A_aHL, ADD_A_A, ADC_A_C, ADC_A_C, ADC_A_C, ADC_A_E, ADC_A_H, ADC_A_L, ADC_A_aHL, ADC_A_A,
                                SUB_B, SUB_C, SUB_D, SUB_E, SUB_H, SUB_L, SUB_aHL, SUB_A, SBC_A_B, SBC_A_C, SBC_A_D, SBC_A_E, SBC_A_H, SBC_A_L, SBC_A_aHL, SBC_A_A,
                                AND_B, AND_C, AND_D, AND_E, AND_H, AND_L, AND_aHL, AND_A, XOR_B, XOR_C, XOR_D, XOR_E, XOR_H, XOR_L, XOR_aHL, XOR_A,
                                OR_B, OR_C, OR_D, OR_E, OR_H, OR_L, OR_aHL, OR_A, CP_B, CP_C, CP_D, CP_E, CP_H, CP_L, CP_aHL, CP_A,
                                RET_NZ, POP_BC, JP_NZ_a16, JP_a16, CALL_NZ_a16, PUSH_BC, ADD_A_d8, RST_00H, RET_Z, RET, JP_Z_a16, PREFIX_CB, CALL_Z_a16, CALL_a16, ADC_A_d8, RST_08H, };
        }

        public void Update(GameTime gametime)
        {

        }

        public void nextInstruction()
        {

        }
    }
}
