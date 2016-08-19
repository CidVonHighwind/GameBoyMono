using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyMono
{
    partial class GameBoyCPU
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

            reg_A = 0x00;
            CB_RES_1_A();

            SUB_B();

            Action[] opas = { NOP, LD_BC_d16, LD_aBC_A, INC_BC, INC_B, DEC_B, LD_B_d8, RLCA, LD_a16_SP, ADD_HL_BC, LD_A_aBC, DEC_BC, INC_C, DEC_C, LD_C_d8, RRCA };
            opas[0x00]();
        }

        public void Update(GameTime gametime)
        {

        }

        public void nextInstruction()
        {

        }
    }
}
