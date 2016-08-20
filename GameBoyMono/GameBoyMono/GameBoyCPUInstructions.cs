using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyMono
{
    public partial class GameBoyCPU
    {
        // controll
        public void NOP() { }
        public void STOP() { }
        public void HALT() { }
        public void PREFIX_CB() { }
        public void DI() { }
        public void EI() { }

        // LD 8bit
        public void LD_aBC_A() { generalMemory[reg_BC] = reg_A; }
        public void LD_B_d8() { reg_B = data8; }
        public void LD_A_aBC() { reg_A = getGMemory(reg_BC); }
        public void LD_C_d8() { reg_C = data8; }

        public void LD_aDE_A() { generalMemory[reg_DE] = reg_A; }
        public void LD_D_d8() { reg_D = data8; }
        public void LD_A_aDE() { reg_A = getGMemory(reg_DE); }
        public void LD_E_d8() { reg_E = data8; }

        public void LD_aHLp_A() { generalMemory[reg_HL] = reg_A; } // ToDo
        public void LD_H_d8() { reg_H = data8; }
        public void LD_A_aHLp() { reg_A = getGMemory(reg_DE); } // todo
        public void LD_L_d8() { reg_L = data8; }

        public void LD_aHLm_A() { generalMemory[reg_HL] = reg_A; } // todo
        public void LD_aHL_d8() { generalMemory[reg_HL] = data8; }
        public void LD_A_aHLm() { reg_A = getGMemory(reg_HL); }
        public void LD_A_d8() { reg_A = data8; }

        // 4x
        public void LD_B_B() { }
        public void LD_B_C() { reg_B = reg_C; }
        public void LD_B_D() { reg_B = reg_D; }
        public void LD_B_E() { reg_B = reg_E; }
        public void LD_B_H() { reg_B = reg_H; }
        public void LD_B_L() { reg_B = reg_L; }
        public void LD_B_aHL() { reg_B = getGMemory(reg_HL); }
        public void LD_B_A() { reg_B = reg_A; }
        public void LD_C_B() { reg_C = reg_B; }
        public void LD_C_C() { }
        public void LD_C_D() { reg_C = reg_D; }
        public void LD_C_E() { reg_C = reg_E; }
        public void LD_C_H() { reg_C = reg_H; }
        public void LD_C_L() { reg_C = reg_L; }
        public void LD_C_aHL() { reg_C = getGMemory(reg_HL); }
        public void LD_C_A() { reg_C = reg_A; }

        // 5x
        public void LD_D_B() { reg_D = reg_B; }
        public void LD_D_C() { reg_D = reg_C; }
        public void LD_D_D() { }
        public void LD_D_E() { reg_D = reg_E; }
        public void LD_D_H() { reg_D = reg_H; }
        public void LD_D_L() { reg_D = reg_L; }
        public void LD_D_aHL() { reg_D = getGMemory(reg_HL); }
        public void LD_D_A() { reg_D = reg_A; }
        public void LD_E_B() { reg_E = reg_B; }
        public void LD_E_C() { reg_E = reg_C; }
        public void LD_E_D() { reg_E = reg_D; }
        public void LD_E_E() { }
        public void LD_E_H() { reg_E = reg_H; }
        public void LD_E_L() { reg_E = reg_L; }
        public void LD_E_aHL() { reg_E = getGMemory(reg_HL); }
        public void LD_E_A() { reg_E = reg_A; }

        // 6x
        public void LD_H_B() { reg_H = reg_B; }
        public void LD_H_C() { reg_H = reg_C; }
        public void LD_H_D() { reg_H = reg_D; }
        public void LD_H_E() { reg_H = reg_E; }
        public void LD_H_H() { }
        public void LD_H_L() { reg_H = reg_L; }
        public void LD_H_aHL() { reg_H = getGMemory(reg_HL); }
        public void LD_H_A() { reg_H = reg_A; }
        public void LD_L_B() { reg_L = reg_B; }
        public void LD_L_C() { reg_L = reg_C; }
        public void LD_L_D() { reg_L = reg_D; }
        public void LD_L_E() { reg_L = reg_E; }
        public void LD_L_H() { reg_L = reg_H; }
        public void LD_L_L() { }
        public void LD_L_aHL() { reg_L = getGMemory(reg_HL); }
        public void LD_L_A() { reg_L = reg_A; }

        // 7x
        public void LD_aHL_B() { generalMemory[reg_HL] = reg_B; }
        public void LD_aHL_C() { generalMemory[reg_HL] = reg_C; }
        public void LD_aHL_D() { generalMemory[reg_HL] = reg_D; }
        public void LD_aHL_E() { generalMemory[reg_HL] = reg_E; }
        public void LD_aHL_H() { generalMemory[reg_HL] = reg_H; }
        public void LD_aHL_L() { generalMemory[reg_HL] = reg_L; }
        public void LD_aHL_A() { reg_H = reg_A; }
        public void LD_A_B() { reg_L = reg_B; }
        public void LD_A_C() { reg_L = reg_C; }
        public void LD_A_D() { reg_L = reg_D; }
        public void LD_A_E() { reg_L = reg_H; }
        public void LD_A_H() { reg_L = reg_H; }
        public void LD_A_L() { }
        public void LD_A_aHL() { reg_L = getGMemory(reg_HL); }
        public void LD_A_A() { reg_L = reg_A; }

        // Ex
        public void LDH_a8_A() { generalMemory[(ushort)(0xFF00 + data8)] = reg_A; }
        public void LDH_A_a8() { reg_A = getGMemory((ushort)(0xFF00 + data8)); }

        public void LD_aC_A() { reg_L = reg_A; }
        public void LD_a16_A() { generalMemory[data16] = reg_A; }
        public void LD_A_aC() { reg_A = getGMemory(reg_C); }
        public void LD_A_a16() { reg_A = getGMemory(data16); }

        // LD 16bit
        public void LD_BC_d16() { reg_BC = data16; }
        public void LD_a16_SP() { generalMemory[data16] = (byte)(reg_SP & 0xFF); generalMemory[(ushort)(data16 + 1)] = (byte)(reg_SP >> 8); } // TODO
        public void LD_DE_d16() { reg_DE = data16; }
        public void LD_HL_d16() { reg_HL = data16; }
        public void LD_SP_d16() { reg_SP = data16; }


        public void LD_HL_SPr8() { reg_HL = data16; }
        public void LD_SP_HL() { reg_SP = reg_HL; }

        public void POP_BC() { reg_C = getGMemory(reg_SP++); reg_B = getGMemory(reg_SP++); }
        public void POP_DE() { reg_E = getGMemory(reg_SP++); reg_D = getGMemory(reg_SP++); }
        public void POP_HL() { reg_L = getGMemory(reg_SP++); reg_H = getGMemory(reg_SP++); }
        public void POP_AF() { reg_F = getGMemory(reg_SP++); reg_A = getGMemory(reg_SP++); }

        public void PUSH_BC() { generalMemory[--reg_SP] = reg_B; generalMemory[--reg_SP] = reg_C; }
        public void PUSH_DE() { generalMemory[--reg_SP] = reg_D; generalMemory[--reg_SP] = reg_E; }
        public void PUSH_HL() { generalMemory[--reg_SP] = reg_H; generalMemory[--reg_SP] = reg_L; }
        public void PUSH_AF() { generalMemory[--reg_SP] = reg_A; generalMemory[--reg_SP] = reg_F; }

        // INC 8bit
        public void INC_B() { reg_B++; flag_Z = (reg_B == 0); flag_N = false; flag_H = ((reg_B & 0x1F) == 0x10); }
        public void INC_C() { reg_C++; flag_Z = (reg_C == 0); flag_N = false; flag_H = ((reg_C & 0x1F) == 0x10); }
        public void INC_D() { reg_D++; flag_Z = (reg_D == 0); flag_N = false; flag_H = ((reg_D & 0x1F) == 0x10); }
        public void INC_E() { reg_E++; flag_Z = (reg_E == 0); flag_N = false; flag_H = ((reg_E & 0x1F) == 0x10); }
        public void INC_H() { reg_H++; flag_Z = (reg_H == 0); flag_N = false; flag_H = ((reg_H & 0x1F) == 0x10); }
        public void INC_L() { reg_L++; flag_Z = (reg_L == 0); flag_N = false; flag_H = ((reg_L & 0x1F) == 0x10); }
        public void INC_aHL() { generalMemory[reg_HL]++; flag_Z = (generalMemory[reg_HL] == 0); flag_N = false; flag_H = ((generalMemory[reg_HL] & 0x1F) == 0x10); }
        public void INC_A() { reg_A++; flag_Z = (reg_A == 0); flag_N = false; flag_H = ((reg_A & 0x1F) == 0x10); }

        // INC 16bit
        public void INC_BC() { reg_BC++; }
        public void INC_DE() { reg_DE++; }
        public void INC_HL() { reg_HL++; }
        public void INC_SP() { reg_SP++; }

        // DEC 8bit
        public void DEC_B() { reg_B--; flag_Z = (reg_B == 0); flag_N = true; flag_H = ((reg_B & 0x0F) != 0x0F); } // right???
        public void DEC_D() { reg_D--; flag_Z = (reg_D == 0); flag_N = true; flag_H = ((reg_D & 0x0F) != 0x0F); }
        public void DEC_H() { reg_H--; flag_Z = (reg_H == 0); flag_N = true; flag_H = ((reg_H & 0x0F) != 0x0F); }
        public void DEC_aHL() { generalMemory[reg_HL]--; flag_Z = (generalMemory[reg_HL] == 0); flag_N = true; flag_H = ((generalMemory[reg_HL] & 0x0F) != 0x0F); }
        public void DEC_C() { reg_C--; flag_Z = (reg_C == 0); flag_N = true; flag_H = ((reg_C & 0x0F) != 0x0F); }
        public void DEC_E() { reg_E--; flag_Z = (reg_E == 0); flag_N = true; flag_H = ((reg_E & 0x0F) != 0x0F); }
        public void DEC_L() { reg_L--; flag_Z = (reg_L == 0); flag_N = true; flag_H = ((reg_L & 0x0F) != 0x0F); }
        public void DEC_A() { reg_A--; flag_Z = (reg_A == 0); flag_N = true; flag_H = ((reg_A & 0x0F) != 0x0F); }

        // DEC 16bit
        public void DEC_BC() { reg_BC--; }
        public void DEC_DE() { reg_DE--; }
        public void DEC_HL() { reg_HL--; }
        public void DEC_SP() { reg_SP--; }

        // ADD/ADC 8bit
        public void ADD_A_B() { flag_H = ((reg_A & 0x0F) + (reg_B & 0x0F)) > 0x0F; flag_C = (reg_A + reg_B) > 0xFF; reg_A += reg_B; flag_Z = reg_A == 0x00; flag_N = false; }
        public void ADD_A_C() { flag_H = ((reg_A & 0x0F) + (reg_C & 0x0F)) > 0x0F; flag_C = (reg_A + reg_C) > 0xFF; reg_A += reg_C; flag_Z = reg_A == 0x00; flag_N = false; }
        public void ADD_A_D() { flag_H = ((reg_A & 0x0F) + (reg_D & 0x0F)) > 0x0F; flag_C = (reg_A + reg_D) > 0xFF; reg_A += reg_D; flag_Z = reg_A == 0x00; flag_N = false; }
        public void ADD_A_E() { flag_H = ((reg_A & 0x0F) + (reg_E & 0x0F)) > 0x0F; flag_C = (reg_A + reg_E) > 0xFF; reg_A += reg_E; flag_Z = reg_A == 0x00; flag_N = false; }
        public void ADD_A_H() { flag_H = ((reg_A & 0x0F) + (reg_H & 0x0F)) > 0x0F; flag_C = (reg_A + reg_H) > 0xFF; reg_A += reg_H; flag_Z = reg_A == 0x00; flag_N = false; }
        public void ADD_A_L() { flag_H = ((reg_A & 0x0F) + (reg_L & 0x0F)) > 0x0F; flag_C = (reg_A + reg_L) > 0xFF; reg_A += reg_L; flag_Z = reg_A == 0x00; flag_N = false; }
        public void ADD_A_aHL()
        {
            flag_H = ((reg_A & 0x0F) + (generalMemory[reg_HL] & 0x0F)) > 0x0F;
            flag_C = (reg_A + generalMemory[reg_HL]) > 0xFF; reg_A += generalMemory[reg_HL]; flag_Z = reg_A == 0x00; flag_N = false;
        }
        public void ADD_A_A() { flag_H = ((reg_A & 0x0F) + (reg_A & 0x0F)) > 0x0F; flag_C = (reg_A + reg_A) > 0xFF; reg_A += reg_A; flag_Z = reg_A == 0x00; flag_N = false; }
        public void ADD_A_d8() { flag_H = ((reg_A & 0x0F) + (data8 & 0x0F)) > 0x0F; flag_C = (reg_A + data8) > 0xFF; reg_A += data8; flag_Z = reg_A == 0x00; flag_N = false; }

        int tempData;
        // !!!! cFLAG fail...
        public void ADC_A_B()
        {
            tempData = reg_A + reg_B + (flag_C ? 0x01 : 0x00);
            flag_H = ((reg_A & 0x0F) + (reg_B & 0x0F) + (flag_C ? 0x01 : 0x00)) > 0x0F; flag_C = tempData > 0xFF;
            reg_A = (byte)tempData; flag_Z = reg_A == 0x00; flag_N = false;
        }
        public void ADC_A_C()
        {
            tempData = reg_A + reg_C + (flag_C ? 0x01 : 0x00);
            flag_H = ((reg_A & 0x0F) + (reg_C & 0x0F) + (flag_C ? 0x01 : 0x00)) > 0x0F; flag_C = tempData > 0xFF;
            reg_A = (byte)tempData; flag_Z = reg_A == 0x00; flag_N = false;
        }
        public void ADC_A_D()
        {
            tempData = reg_A + reg_D + (flag_C ? 0x01 : 0x00);
            flag_H = ((reg_A & 0x0F) + (reg_D & 0x0F) + (flag_C ? 0x01 : 0x00)) > 0x0F; flag_C = tempData > 0xFF;
            reg_A = (byte)tempData; flag_Z = reg_A == 0x00; flag_N = false;
        }
        public void ADC_A_E()
        {
            tempData = reg_A + reg_E + (flag_C ? 0x01 : 0x00);
            flag_H = ((reg_A & 0x0F) + (reg_E & 0x0F) + (flag_C ? 0x01 : 0x00)) > 0x0F; flag_C = tempData > 0xFF;
            reg_A = (byte)tempData; flag_Z = reg_A == 0x00; flag_N = false;
        }
        public void ADC_A_H()
        {
            tempData = reg_A + reg_H + (flag_C ? 0x01 : 0x00);
            flag_H = ((reg_A & 0x0F) + (reg_H & 0x0F) + (flag_C ? 0x01 : 0x00)) > 0x0F; flag_C = tempData > 0xFF;
            reg_A = (byte)tempData; flag_Z = reg_A == 0x00; flag_N = false;
        }
        public void ADC_A_L()
        {
            tempData = reg_A + reg_L + (flag_C ? 0x01 : 0x00);
            flag_H = ((reg_A & 0x0F) + (reg_L & 0x0F) + (flag_C ? 0x01 : 0x00)) > 0x0F; flag_C = tempData > 0xFF;
            reg_A = (byte)tempData; flag_Z = reg_A == 0x00; flag_N = false;
        }
        public void ADC_A_aHL()
        {
            tempData = reg_A + generalMemory[reg_HL] + (flag_C ? 0x01 : 0x00);
            flag_H = ((reg_A & 0x0F) + (generalMemory[reg_HL] & 0x0F) + (flag_C ? 0x01 : 0x00)) > 0x0F; flag_C = tempData > 0xFF;
            reg_A = (byte)tempData; flag_Z = reg_A == 0x00; flag_N = false;
        }
        public void ADC_A_A()
        {
            tempData = reg_A + reg_A + (flag_C ? 0x01 : 0x00);
            flag_H = ((reg_A & 0x0F) + (reg_A & 0x0F) + (flag_C ? 0x01 : 0x00)) > 0x0F; flag_C = tempData > 0xFF;
            reg_A = (byte)tempData; flag_Z = reg_A == 0x00; flag_N = false;
        }
        public void ADC_A_d8()
        {
            tempData = reg_A + data8 + (flag_C ? 0x01 : 0x00);
            flag_H = ((reg_A & 0x0F) + (data8 & 0x0F) + (flag_C ? 0x01 : 0x00)) > 0x0F; flag_C = tempData > 0xFF;
            reg_A = (byte)tempData; flag_Z = reg_A == 0x00; flag_N = false;
        }

        // ADD 16bit
        public void ADD_HL_BC() { flag_N = false; flag_H = ((reg_HL & 0x0FFF) + (reg_BC & 0x0FFF)) > 0x0FFF; flag_C = (reg_HL + reg_BC) > 0xFFFF; reg_HL += reg_BC; }
        public void ADD_HL_DE() { flag_N = false; flag_H = ((reg_HL & 0x0FFF) + (reg_DE & 0x0FFF)) > 0x0FFF; flag_C = (reg_HL + reg_DE) > 0xFFFF; reg_HL += reg_DE; }
        public void ADD_HL_HL() { flag_N = false; flag_H = ((reg_HL & 0x0FFF) + (reg_HL & 0x0FFF)) > 0x0FFF; flag_C = (reg_HL + reg_HL) > 0xFFFF; reg_HL += reg_HL; }
        public void ADD_HL_SP() { flag_N = false; flag_H = ((reg_HL & 0x0FFF) + (reg_SP & 0x0FFF)) > 0x0FFF; flag_C = (reg_HL + reg_SP) > 0xFFFF; reg_HL += reg_SP; }

        // SUB 8bit
        public void SUB_B() { flag_H = (reg_A & 0x0F) >= (reg_B & 0x0F); flag_C = reg_A >= reg_B; reg_A = (byte)(reg_A - reg_B); flag_Z = reg_A == 0x00; flag_N = true; }
        public void SUB_C() { flag_H = (reg_A & 0x0F) >= (reg_C & 0x0F); flag_C = reg_A >= reg_C; reg_A = (byte)(reg_A - reg_C); flag_Z = reg_A == 0x00; flag_N = true; }
        public void SUB_D() { flag_H = (reg_A & 0x0F) >= (reg_D & 0x0F); flag_C = reg_A >= reg_D; reg_A = (byte)(reg_A - reg_D); flag_Z = reg_A == 0x00; flag_N = true; }
        public void SUB_E() { flag_H = (reg_A & 0x0F) >= (reg_E & 0x0F); flag_C = reg_A >= reg_E; reg_A = (byte)(reg_A - reg_E); flag_Z = reg_A == 0x00; flag_N = true; }
        public void SUB_H() { flag_H = (reg_A & 0x0F) >= (reg_H & 0x0F); flag_C = reg_A >= reg_H; reg_A = (byte)(reg_A - reg_H); flag_Z = reg_A == 0x00; flag_N = true; }
        public void SUB_L() { flag_H = (reg_A & 0x0F) >= (reg_L & 0x0F); flag_C = reg_A >= reg_L; reg_A = (byte)(reg_A - reg_L); flag_Z = reg_A == 0x00; flag_N = true; }
        public void SUB_aHL()
        {
            flag_H = (reg_A & 0x0F) >= (generalMemory[reg_HL] & 0x0F);
            flag_C = reg_A >= generalMemory[reg_HL]; reg_A = (byte)(reg_A - generalMemory[reg_HL]); flag_Z = reg_A == 0x00; flag_N = true;
        }
        public void SUB_A() { flag_H = (reg_A & 0x0F) >= (reg_A & 0x0F); flag_C = true; reg_A = 0; flag_Z = reg_A == 0x00; flag_N = true; }
        public void SUB_d8() { flag_H = (reg_A & 0x0F) >= (data8 & 0x0F); flag_C = reg_A >= data8; reg_A = (byte)(reg_A - data8); flag_Z = reg_A == 0x00; flag_N = true; }

        // AND 8bit
        public void AND_B() { reg_A &= reg_B; flag_Z = reg_A == 0x00; flag_N = false; flag_H = true; flag_C = false; }
        public void AND_C() { reg_A &= reg_C; flag_Z = reg_A == 0x00; flag_N = false; flag_H = true; flag_C = false; }
        public void AND_D() { reg_A &= reg_D; flag_Z = reg_A == 0x00; flag_N = false; flag_H = true; flag_C = false; }
        public void AND_E() { reg_A &= reg_E; flag_Z = reg_A == 0x00; flag_N = false; flag_H = true; flag_C = false; }
        public void AND_H() { reg_A &= reg_H; flag_Z = reg_A == 0x00; flag_N = false; flag_H = true; flag_C = false; }
        public void AND_L() { reg_A &= reg_L; flag_Z = reg_A == 0x00; flag_N = false; flag_H = true; flag_C = false; }
        public void AND_aHL() { reg_A &= generalMemory[reg_HL]; flag_Z = reg_A == 0x00; flag_N = false; flag_H = true; flag_C = false; }
        public void AND_A() { reg_A &= reg_A; flag_Z = reg_A == 0x00; flag_N = false; flag_H = true; flag_C = false; }
        public void AND_d8() { reg_A &= data8; flag_Z = reg_A == 0x00; flag_N = false; flag_H = true; flag_C = false; }

        // OR 8bit
        public void OR_B() { reg_A |= reg_B; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void OR_C() { reg_A |= reg_C; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void OR_D() { reg_A |= reg_D; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void OR_E() { reg_A |= reg_E; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void OR_H() { reg_A |= reg_H; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void OR_L() { reg_A |= reg_L; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void OR_aHL() { reg_A |= generalMemory[reg_HL]; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void OR_A() { reg_A |= reg_A; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void OR_d8() { reg_A |= data8; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }

        // XOR 8bit
        public void XOR_B() { reg_A ^= reg_B; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void XOR_C() { reg_A ^= reg_C; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void XOR_D() { reg_A ^= reg_D; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void XOR_E() { reg_A ^= reg_E; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void XOR_H() { reg_A ^= reg_H; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void XOR_L() { reg_A ^= reg_L; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void XOR_aHL() { reg_A ^= generalMemory[reg_HL]; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void XOR_A() { reg_A ^= reg_A; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }
        public void XOR_d8() { reg_A ^= data8; flag_Z = reg_A == 0x00; flag_N = false; flag_H = false; flag_C = false; }

        // CP
        public void CP_B() { flag_Z = (reg_A - reg_B) == 0x00; flag_N = true; flag_H = ((reg_A - reg_B) & 0x0F) > (reg_A & 0x0F); flag_C = reg_A < reg_B; }
        public void CP_C() { flag_Z = (reg_A - reg_C) == 0x00; flag_N = true; flag_H = ((reg_A - reg_C) & 0x0F) > (reg_A & 0x0F); flag_C = reg_A < reg_C; }
        public void CP_D() { flag_Z = (reg_A - reg_D) == 0x00; flag_N = true; flag_H = ((reg_A - reg_D) & 0x0F) > (reg_A & 0x0F); flag_C = reg_A < reg_D; }
        public void CP_E() { flag_Z = (reg_A - reg_E) == 0x00; flag_N = true; flag_H = ((reg_A - reg_E) & 0x0F) > (reg_A & 0x0F); flag_C = reg_A < reg_E; }
        public void CP_H() { flag_Z = (reg_A - reg_H) == 0x00; flag_N = true; flag_H = ((reg_A - reg_H) & 0x0F) > (reg_A & 0x0F); flag_C = reg_A < reg_H; }
        public void CP_L() { flag_Z = (reg_A - reg_L) == 0x00; flag_N = true; flag_H = ((reg_A - reg_L) & 0x0F) > (reg_A & 0x0F); flag_C = reg_A < reg_L; }
        public void CP_aHL() { flag_Z = (reg_A - generalMemory[reg_HL]) == 0x00; flag_N = true; flag_H = ((reg_A - generalMemory[reg_HL]) & 0x0F) > (reg_A & 0x0F); flag_C = reg_A < generalMemory[reg_HL]; }
        public void CP_A() { flag_Z = true; flag_N = true; flag_H = false; flag_C = false; }
        public void CP_d8() { flag_Z = (reg_A - data8) == 0x00; flag_N = true; flag_H = ((reg_A - data8) & 0x0F) > (reg_A & 0x0F); flag_C = reg_A < data8; }

        // very wrong
        //public void DAA() { reg_A = (byte)(((reg_A / 10) << 4) + reg_A % 10); flag_Z = reg_A == 0x00; flag_H = false; }

        // RLCA
        public void RLCA() { tempData = reg_A; reg_A = (byte)(reg_A << 1); flag_Z = (reg_A == 0); flag_N = false; flag_H = false; flag_C = tempData > 0x7F; }
        public void RLC() { tempData = reg_A; reg_A = (byte)((reg_A << 1) | (flag_C ? 0x01 : 0x00)); flag_Z = (reg_A == 0); flag_N = false; flag_H = false; flag_C = tempData > 0x7F; }
        public void RRCA() { tempData = reg_A; reg_A = (byte)(reg_A >> 1); flag_Z = (reg_A == 0); flag_N = false; flag_H = false; flag_C = (tempData & 0x01) == 1; }
        public void RRA() { tempData = reg_A; reg_A = (byte)((reg_A >> 1) | (flag_C ? 0x80 : 0x00)); flag_Z = (reg_A == 0); flag_N = false; flag_H = false; flag_C = (tempData & 0x01) == 1; }

        // JP/JR
        public void JP_a16() { reg_PC = data16; }
        public void JP_aHL() { reg_PC = generalMemory[reg_HL]; }
        public void JP_NZ_a16() { if (!flag_Z) reg_PC = data16; }
        public void JP_Z_a16() { if (flag_Z) reg_PC = data16; }
        public void JP_NC_a16() { if (!flag_C) reg_PC = data16; }
        public void JP_C_a16() { if (flag_C) reg_PC = data16; }

        public void JR_d8() { reg_PC += data8; }
        public void JR_NZ_a8() { if (!flag_Z) reg_PC += data8; }
        public void JR_Z_a8() { if (flag_Z) reg_PC += data8; }
        public void JR_NC_a8() { if (!flag_C) reg_PC += data8; }
        public void JR_C_a8() { if (flag_C) reg_PC += data8; }

        // Retrurns
        public void RET() { tempData = getGMemory(reg_SP++); tempData += getGMemory(reg_SP++) << 8; reg_PC = (ushort)tempData; }

        // CB instructions
        // RES
        public void CB_RES_0_B() { reg_B &= 0xFE; }
        public void CB_RES_0_C() { reg_C &= 0xFE; }
        public void CB_RES_0_D() { reg_D &= 0xFE; }
        public void CB_RES_0_E() { reg_E &= 0xFE; }
        public void CB_RES_0_H() { reg_H &= 0xFE; }
        public void CB_RES_0_L() { reg_L &= 0xFE; }
        public void CB_RES_0_aHL() { generalMemory[reg_HL] &= 0xFE; }
        public void CB_RES_0_A() { reg_A &= 0xFE; }

        public void CB_RES_1_B() { reg_B &= 0xFD; }
        public void CB_RES_1_C() { reg_C &= 0xFD; }
        public void CB_RES_1_D() { reg_D &= 0xFD; }
        public void CB_RES_1_E() { reg_E &= 0xFD; }
        public void CB_RES_1_H() { reg_H &= 0xFD; }
        public void CB_RES_1_L() { reg_L &= 0xFD; }
        public void CB_RES_1_aHL() { generalMemory[reg_HL] &= 0xFD; }
        public void CB_RES_1_A() { reg_A &= 0xFD; }

        public void CB_RES_2_B() { reg_B &= 0xFB; }
        public void CB_RES_2_C() { reg_C &= 0xFB; }
        public void CB_RES_2_D() { reg_D &= 0xFB; }
        public void CB_RES_2_E() { reg_E &= 0xFB; }
        public void CB_RES_2_H() { reg_H &= 0xFB; }
        public void CB_RES_2_L() { reg_L &= 0xFB; }
        public void CB_RES_2_aHL() { generalMemory[reg_HL] &= 0xFB; }
        public void CB_RES_2_A() { reg_A &= 0xFB; }

        public void CB_RES_3_B() { reg_B &= 0xF7; }
        public void CB_RES_3_C() { reg_C &= 0xF7; }
        public void CB_RES_3_D() { reg_D &= 0xF7; }
        public void CB_RES_3_E() { reg_E &= 0xF7; }
        public void CB_RES_3_H() { reg_H &= 0xF7; }
        public void CB_RES_3_L() { reg_L &= 0xF7; }
        public void CB_RES_3_aHL() { generalMemory[reg_HL] &= 0xF7; }
        public void CB_RES_3_A() { reg_A &= 0xF7; }

        public void CB_RES_4_B() { reg_B &= 0xEF; }
        public void CB_RES_4_C() { reg_C &= 0xEF; }
        public void CB_RES_4_D() { reg_D &= 0xEF; }
        public void CB_RES_4_E() { reg_E &= 0xEF; }
        public void CB_RES_4_H() { reg_H &= 0xEF; }
        public void CB_RES_4_L() { reg_L &= 0xEF; }
        public void CB_RES_4_aHL() { generalMemory[reg_HL] &= 0xEF; }
        public void CB_RES_4_A() { reg_A &= 0xEF; }

        public void CB_RES_5_B() { reg_B &= 0xDF; }
        public void CB_RES_5_C() { reg_C &= 0xDF; }
        public void CB_RES_5_D() { reg_D &= 0xDF; }
        public void CB_RES_5_E() { reg_E &= 0xDF; }
        public void CB_RES_5_H() { reg_H &= 0xDF; }
        public void CB_RES_5_L() { reg_L &= 0xDF; }
        public void CB_RES_5_aHL() { generalMemory[reg_HL] &= 0xDF; }
        public void CB_RES_5_A() { reg_A &= 0xDF; }

        public void CB_RES_6_B() { reg_B &= 0xBF; }
        public void CB_RES_6_C() { reg_C &= 0xBF; }
        public void CB_RES_6_D() { reg_D &= 0xBF; }
        public void CB_RES_6_E() { reg_E &= 0xBF; }
        public void CB_RES_6_H() { reg_H &= 0xBF; }
        public void CB_RES_6_L() { reg_L &= 0xBF; }
        public void CB_RES_6_aHL() { generalMemory[reg_HL] &= 0xBF; }
        public void CB_RES_6_A() { reg_A &= 0xBF; }

        public void CB_RES_7_B() { reg_B &= 0x7F; }
        public void CB_RES_7_C() { reg_C &= 0x7F; }
        public void CB_RES_7_D() { reg_D &= 0x7F; }
        public void CB_RES_7_E() { reg_E &= 0x7F; }
        public void CB_RES_7_H() { reg_H &= 0x7F; }
        public void CB_RES_7_L() { reg_L &= 0x7F; }
        public void CB_RES_7_aHL() { generalMemory[reg_HL] &= 0x7F; }
        public void CB_RES_7_A() { reg_A &= 0x7F; }

        // SET
        public void CB_SET_0_B() { reg_B |= 0x01; }
        public void CB_SET_0_C() { reg_C |= 0x01; }
        public void CB_SET_0_D() { reg_D |= 0x01; }
        public void CB_SET_0_E() { reg_E |= 0x01; }
        public void CB_SET_0_H() { reg_H |= 0x01; }
        public void CB_SET_0_L() { reg_L |= 0x01; }
        public void CB_SET_0_aHL() { generalMemory[reg_HL] |= 0x01; }
        public void CB_SET_0_A() { reg_A |= 0x01; }

        public void CB_SET_1_B() { reg_B |= 0x02; }
        public void CB_SET_1_C() { reg_C |= 0x02; }
        public void CB_SET_1_D() { reg_D |= 0x02; }
        public void CB_SET_1_E() { reg_E |= 0x02; }
        public void CB_SET_1_H() { reg_H |= 0x02; }
        public void CB_SET_1_L() { reg_L |= 0x02; }
        public void CB_SET_1_aHL() { generalMemory[reg_HL] |= 0x02; }
        public void CB_SET_1_A() { reg_A |= 0x02; }

        public void CB_SET_2_B() { reg_B |= 0x04; }
        public void CB_SET_2_C() { reg_C |= 0x04; }
        public void CB_SET_2_D() { reg_D |= 0x04; }
        public void CB_SET_2_E() { reg_E |= 0x04; }
        public void CB_SET_2_H() { reg_H |= 0x04; }
        public void CB_SET_2_L() { reg_L |= 0x04; }
        public void CB_SET_2_aHL() { generalMemory[reg_HL] |= 0x04; }
        public void CB_SET_2_A() { reg_A |= 0x04; }

        public void CB_SET_3_B() { reg_B |= 0x08; }
        public void CB_SET_3_C() { reg_C |= 0x08; }
        public void CB_SET_3_D() { reg_D |= 0x08; }
        public void CB_SET_3_E() { reg_E |= 0x08; }
        public void CB_SET_3_H() { reg_H |= 0x08; }
        public void CB_SET_3_L() { reg_L |= 0x08; }
        public void CB_SET_3_aHL() { generalMemory[reg_HL] |= 0x08; }
        public void CB_SET_3_A() { reg_A |= 0x08; }

        public void CB_SET_4_B() { reg_B |= 0x10; }
        public void CB_SET_4_C() { reg_C |= 0x10; }
        public void CB_SET_4_D() { reg_D |= 0x10; }
        public void CB_SET_4_E() { reg_E |= 0x10; }
        public void CB_SET_4_H() { reg_H |= 0x10; }
        public void CB_SET_4_L() { reg_L |= 0x10; }
        public void CB_SET_4_aHL() { generalMemory[reg_HL] |= 0x10; }
        public void CB_SET_4_A() { reg_A |= 0x10; }

        public void CB_SET_5_B() { reg_B |= 0x20; }
        public void CB_SET_5_C() { reg_C |= 0x20; }
        public void CB_SET_5_D() { reg_D |= 0x20; }
        public void CB_SET_5_E() { reg_E |= 0x20; }
        public void CB_SET_5_H() { reg_H |= 0x20; }
        public void CB_SET_5_L() { reg_L |= 0x20; }
        public void CB_SET_5_aHL() { generalMemory[reg_HL] |= 0x20; }
        public void CB_SET_5_A() { reg_A |= 0x20; }

        public void CB_SET_6_B() { reg_B |= 0x40; }
        public void CB_SET_6_C() { reg_C |= 0x40; }
        public void CB_SET_6_D() { reg_D |= 0x40; }
        public void CB_SET_6_E() { reg_E |= 0x40; }
        public void CB_SET_6_H() { reg_H |= 0x40; }
        public void CB_SET_6_L() { reg_L |= 0x40; }
        public void CB_SET_6_aHL() { generalMemory[reg_HL] |= 0x40; }
        public void CB_SET_6_A() { reg_A |= 0x40; }

        public void CB_SET_7_B() { reg_B |= 0x80; }
        public void CB_SET_7_C() { reg_C |= 0x80; }
        public void CB_SET_7_D() { reg_D |= 0x80; }
        public void CB_SET_7_E() { reg_E |= 0x80; }
        public void CB_SET_7_H() { reg_H |= 0x80; }
        public void CB_SET_7_L() { reg_L |= 0x80; }
        public void CB_SET_7_aHL() { generalMemory[reg_HL] |= 0x80; }
        public void CB_SET_7_A() { reg_A |= 0x80; }

        // BIT
        // redo without ! and with == 0
        public void CB_BIT_0_B() { flag_Z = !((reg_B & 0x01) == 0x01); flag_N = false; flag_H = true; }
        public void CB_BIT_0_C() { flag_Z = !((reg_C & 0x01) == 0x01); flag_N = false; flag_H = true; }
        public void CB_BIT_0_D() { flag_Z = !((reg_D & 0x01) == 0x01); flag_N = false; flag_H = true; }
        public void CB_BIT_0_E() { flag_Z = !((reg_E & 0x01) == 0x01); flag_N = false; flag_H = true; }
        public void CB_BIT_0_H() { flag_Z = !((reg_H & 0x01) == 0x01); flag_N = false; flag_H = true; }
        public void CB_BIT_0_L() { flag_Z = !((reg_L & 0x01) == 0x01); flag_N = false; flag_H = true; }
        public void CB_BIT_0_aHL() { flag_Z = !((generalMemory[reg_HL] & 0x01) == 0x01); flag_N = false; flag_H = true; }
        public void CB_BIT_0_A() { flag_Z = !((reg_A & 0x01) == 0x01); flag_N = false; flag_H = true; }

        public void CB_BIT_1_B() { flag_Z = !((reg_B & 0x02) == 0x02); flag_N = false; flag_H = true; }
        public void CB_BIT_1_C() { flag_Z = !((reg_C & 0x02) == 0x02); flag_N = false; flag_H = true; }
        public void CB_BIT_1_D() { flag_Z = !((reg_D & 0x02) == 0x02); flag_N = false; flag_H = true; }
        public void CB_BIT_1_E() { flag_Z = !((reg_E & 0x02) == 0x02); flag_N = false; flag_H = true; }
        public void CB_BIT_1_H() { flag_Z = !((reg_H & 0x02) == 0x02); flag_N = false; flag_H = true; }
        public void CB_BIT_1_L() { flag_Z = !((reg_L & 0x02) == 0x02); flag_N = false; flag_H = true; }
        public void CB_BIT_1_aHL() { flag_Z = !((generalMemory[reg_HL] & 0x02) == 0x02); flag_N = false; flag_H = true; }
        public void CB_BIT_1_A() { flag_Z = !((reg_A & 0x02) == 0x02); flag_N = false; flag_H = true; }

        public void CB_BIT_2_B() { flag_Z = !((reg_B & 0x04) == 0x04); flag_N = false; flag_H = true; }
        public void CB_BIT_2_C() { flag_Z = !((reg_C & 0x04) == 0x04); flag_N = false; flag_H = true; }
        public void CB_BIT_2_D() { flag_Z = !((reg_D & 0x04) == 0x04); flag_N = false; flag_H = true; }
        public void CB_BIT_2_E() { flag_Z = !((reg_E & 0x04) == 0x04); flag_N = false; flag_H = true; }
        public void CB_BIT_2_H() { flag_Z = !((reg_H & 0x04) == 0x04); flag_N = false; flag_H = true; }
        public void CB_BIT_2_L() { flag_Z = !((reg_L & 0x04) == 0x04); flag_N = false; flag_H = true; }
        public void CB_BIT_2_aHL() { flag_Z = !((generalMemory[reg_HL] & 0x04) == 0x04); flag_N = false; flag_H = true; }
        public void CB_BIT_2_A() { flag_Z = !((reg_A & 0x04) == 0x04); flag_N = false; flag_H = true; }

        public void CB_BIT_3_B() { flag_Z = !((reg_B & 0x08) == 0x08); flag_N = false; flag_H = true; }
        public void CB_BIT_3_C() { flag_Z = !((reg_C & 0x08) == 0x08); flag_N = false; flag_H = true; }
        public void CB_BIT_3_D() { flag_Z = !((reg_D & 0x08) == 0x08); flag_N = false; flag_H = true; }
        public void CB_BIT_3_E() { flag_Z = !((reg_E & 0x08) == 0x08); flag_N = false; flag_H = true; }
        public void CB_BIT_3_H() { flag_Z = !((reg_H & 0x08) == 0x08); flag_N = false; flag_H = true; }
        public void CB_BIT_3_L() { flag_Z = !((reg_L & 0x08) == 0x08); flag_N = false; flag_H = true; }
        public void CB_BIT_3_aHL() { flag_Z = !((generalMemory[reg_HL] & 0x08) == 0x08); flag_N = false; flag_H = true; }
        public void CB_BIT_3_A() { flag_Z = !((reg_A & 0x08) == 0x08); flag_N = false; flag_H = true; }

        public void CB_BIT_4_B() { flag_Z = !((reg_B & 0x10) == 0x10); flag_N = false; flag_H = true; }
        public void CB_BIT_4_C() { flag_Z = !((reg_C & 0x10) == 0x10); flag_N = false; flag_H = true; }
        public void CB_BIT_4_D() { flag_Z = !((reg_D & 0x10) == 0x10); flag_N = false; flag_H = true; }
        public void CB_BIT_4_E() { flag_Z = !((reg_E & 0x10) == 0x10); flag_N = false; flag_H = true; }
        public void CB_BIT_4_H() { flag_Z = !((reg_H & 0x10) == 0x10); flag_N = false; flag_H = true; }
        public void CB_BIT_4_L() { flag_Z = !((reg_L & 0x10) == 0x10); flag_N = false; flag_H = true; }
        public void CB_BIT_4_aHL() { flag_Z = !((generalMemory[reg_HL] & 0x10) == 0x10); flag_N = false; flag_H = true; }
        public void CB_BIT_4_A() { flag_Z = !((reg_A & 0x10) == 0x10); flag_N = false; flag_H = true; }

        public void CB_BIT_5_B() { flag_Z = !((reg_B & 0x20) == 0x20); flag_N = false; flag_H = true; }
        public void CB_BIT_5_C() { flag_Z = !((reg_C & 0x20) == 0x20); flag_N = false; flag_H = true; }
        public void CB_BIT_5_D() { flag_Z = !((reg_D & 0x20) == 0x20); flag_N = false; flag_H = true; }
        public void CB_BIT_5_E() { flag_Z = !((reg_E & 0x20) == 0x20); flag_N = false; flag_H = true; }
        public void CB_BIT_5_H() { flag_Z = !((reg_H & 0x20) == 0x20); flag_N = false; flag_H = true; }
        public void CB_BIT_5_L() { flag_Z = !((reg_L & 0x20) == 0x20); flag_N = false; flag_H = true; }
        public void CB_BIT_5_aHL() { flag_Z = !((generalMemory[reg_HL] & 0x20) == 0x20); flag_N = false; flag_H = true; }
        public void CB_BIT_5_A() { flag_Z = !((reg_A & 0x20) == 0x20); flag_N = false; flag_H = true; }

        public void CB_BIT_6_B() { flag_Z = !((reg_B & 0x40) == 0x40); flag_N = false; flag_H = true; }
        public void CB_BIT_6_C() { flag_Z = !((reg_C & 0x40) == 0x40); flag_N = false; flag_H = true; }
        public void CB_BIT_6_D() { flag_Z = !((reg_D & 0x40) == 0x40); flag_N = false; flag_H = true; }
        public void CB_BIT_6_E() { flag_Z = !((reg_E & 0x40) == 0x40); flag_N = false; flag_H = true; }
        public void CB_BIT_6_H() { flag_Z = !((reg_H & 0x40) == 0x40); flag_N = false; flag_H = true; }
        public void CB_BIT_6_L() { flag_Z = !((reg_L & 0x40) == 0x40); flag_N = false; flag_H = true; }
        public void CB_BIT_6_aHL() { flag_Z = !((generalMemory[reg_HL] & 0x40) == 0x40); flag_N = false; flag_H = true; }
        public void CB_BIT_6_A() { flag_Z = !((reg_A & 0x40) == 0x40); flag_N = false; flag_H = true; }

        public void CB_BIT_7_B() { flag_Z = !((reg_B & 0x80) == 0x80); flag_N = false; flag_H = true; }
        public void CB_BIT_7_C() { flag_Z = !((reg_C & 0x80) == 0x80); flag_N = false; flag_H = true; }
        public void CB_BIT_7_D() { flag_Z = !((reg_D & 0x80) == 0x80); flag_N = false; flag_H = true; }
        public void CB_BIT_7_E() { flag_Z = !((reg_E & 0x80) == 0x80); flag_N = false; flag_H = true; }
        public void CB_BIT_7_H() { flag_Z = !((reg_H & 0x80) == 0x80); flag_N = false; flag_H = true; }
        public void CB_BIT_7_L() { flag_Z = !((reg_L & 0x80) == 0x80); flag_N = false; flag_H = true; }
        public void CB_BIT_7_aHL() { flag_Z = !((generalMemory[reg_HL] & 0x80) == 0x80); flag_N = false; flag_H = true; }
        public void CB_BIT_7_A() { flag_Z = !((reg_A & 0x80) == 0x80); flag_N = false; flag_H = true; }
    }
}
