using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyMono
{
    public class Sound
    {
        BufferedWaveProvider bufferProvider;
        WaveOut waveOut;

        const int outputRate = 44100;


        public byte[] soundBuffer = new byte[1470];
        int currentByte;

        // FF10-FF26
        byte[] soundRegister = new byte[0x2F];
        // sweep

        // mode 1
        // FF11
        byte modeOneSoundLengh;
        byte modeOneWavePatternDuty;
        float modeOneWavePatternDutyPercentage;
        // FF12
        byte modeOneEnvelopeSweeps;
        bool modeOneEnvelopeInc;
        byte modeOneEnvelopeInitValue;

        int modeOneEnvelopeWatch;
        int modeOneEnvelopeCounter;
        int modeOneEnvelopeState;

        // FF13-FF14
        short modeOneFrequency;
        // FF14
        bool modeOnecounterConsecutiveSelection;
        bool modeOneInit;


        int modeOneFreqWatch;
        short waveOne = 5000;

        bool modeOneFreqDuty;
        int modeOneFreqTime;

        int modeOneSoundTime;

        int modeOneWatch;
        bool modeOneCounterEnable;


        // mode 2
        // FF16
        byte modeTwoSoundLengh;
        byte modeTwoWavePatternDuty;
        float modeTwoWavePatternDutyPercentage;
        // FF17
        byte modeTwoEnvelopeSweeps;
        bool modeTwoEnvelopeInc;
        byte modeTwoEnvelopeInitValue;
        // FF18-FF19
        short modeTwoFrequency;
        // FF19
        bool modeTwoCounterConsecutiveSelection;
        bool modeTwoInit;

        int modeTwoEnvelopeWatch;
        int modeTwoEnvelopeCounter;
        int modeTwoEnvelopeState;

        bool modeTwoFreqDuty;
        int modeTwoFreqTime;

        int modeTwoSoundTime;

        int modeTwoWatch;
        bool modeTwoCounterEnable;

        int modeTwoFreqWatch;
        short waveTwo = 5000;


        // FF24
        byte soundOutputLevel;
        // FF25
        byte soundOutput;
        // FF26
        byte soundOnOff;

        public Sound()
        {
            bufferProvider = new BufferedWaveProvider(new WaveFormat(outputRate, 16, 1));

            waveOut = new WaveOut();

            // 44100/60*2 = 
            waveOut.Init(bufferProvider);
        }

        public void Play()
        {
            waveOut.Play();
        }

        public void Stop()
        {
            waveOut.Stop();
        }

        public void Exit()
        {
            waveOut.Stop();
            waveOut.Dispose();
        }

        public void AddCurrentBuffer()
        {
            bufferProvider.AddSamples(soundBuffer, 0, soundBuffer.Length);

            currentByte = 0;
        }

        public byte this[int index]
        {
            get
            {
                // MODE ONE
                if (index == 0xFF10) { }
                else if (index == 0xFF11)
                {
                    return (byte)(modeOneWavePatternDuty << 6);
                }
                else if (index == 0xFF12)
                {
                    return soundRegister[index - 0xFF10];
                }

                // MODE TWO
                else if (index == 0xFF16)
                {
                    return (byte)(modeTwoWavePatternDuty << 6);
                }
                else if (index == 0xFF17)
                {
                    return soundRegister[index - 0xFF10];
                }

                // Stuff
                else if (index == 0xFF24)
                    return soundOutputLevel;
                else if (index == 0xFF25)
                    return soundOutput;
                else if (index == 0xFF26)
                    return soundOnOff;

                return soundRegister[index - 0xFF10];
            }
            set
            {

                if (index == 0xFF10)
                {
                    soundRegister[index - 0xFF10] = value;
                    if (value != 0)
                    {

                    }
                }
                else if (index == 0xFF11)
                {
                    soundRegister[index - 0xFF10] = value;
                    
                    modeOneWavePatternDuty = (byte)(value >> 6);

                    // 12.5, 25, 50, 75
                    modeOneWavePatternDutyPercentage = 25 * modeOneWavePatternDuty + ((modeOneWavePatternDuty == 0) ? 12.5f : 0);

                    // Sound Length = (64-t1) * (1/256) sec
                    modeOneSoundTime = (int)(((64 - (byte)(value & 0x07)) / 256d) * outputRate);
                }
                else if (index == 0xFF12)
                {
                    soundRegister[index - 0xFF10] = value;

                    modeOneEnvelopeSweeps = (byte)(value & 0x07);
                    modeOneEnvelopeInc = (value & 0x08) == 0x08;
                    modeOneEnvelopeInitValue = (byte)(value >> 4);

                    modeOneEnvelopeState = modeOneEnvelopeInitValue;
                }
                else if (index == 0xFF13)
                {
                    soundRegister[index - 0xFF10] = value;

                    modeOneFrequency = (short)((modeOneFrequency & 0xF00) + value);
                    modeOneFreqTime = (int)(outputRate / (4194304 / (double)(32 * (2048 - modeOneFrequency))));
                }
                else if (index == 0xFF14)
                {
                    soundRegister[index - 0xFF10] = value;

                    modeOneFrequency = (short)((modeOneFrequency & 0xFF) + ((value & 0x07) << 8));
                    modeOneFreqTime = (int)(outputRate / (4194304 / (double)(32 * (2048 - modeOneFrequency))));

                    modeOnecounterConsecutiveSelection = (value & 0x40) == 0x40;

                    modeOneCounterEnable = (value & 0x40) == 0x40;

                    if ((value & 0x80) == 0x80)
                    {
                        modeOneInit = true;

                        soundOnOff |= 0x01;
                        modeOneWatch = 0;

                        modeOneEnvelopeState = modeOneEnvelopeInitValue;
                    }
                }

                // MODE TWO
                else if (index == 0xFF16)
                {
                    soundRegister[index - 0xFF10] = value;

                    modeTwoSoundLengh = (byte)(value & 0x07);
                    modeTwoWavePatternDuty = (byte)(value >> 6);

                    // 12.5, 25, 50, 75
                    modeTwoWavePatternDutyPercentage = 25 * modeTwoWavePatternDuty + ((modeTwoWavePatternDuty == 0) ? 12.5f : 0);
                    
                    // Sound Length = (64-t1) * (1/256) sec
                    modeTwoSoundTime = (int)(((64 - modeTwoSoundLengh) / 256d) * outputRate);
                }
                else if (index == 0xFF17)
                {
                    soundRegister[index - 0xFF10] = value;

                    modeTwoEnvelopeSweeps = (byte)(value & 0x07);
                    modeTwoEnvelopeInc = (value & 0x08) == 0x08;
                    modeTwoEnvelopeInitValue = (byte)(value >> 4);

                    modeTwoEnvelopeState = modeTwoEnvelopeInitValue;
                }
                else if (index == 0xFF18)
                {
                    soundRegister[index - 0xFF10] = value;

                    modeTwoFrequency = (short)((modeTwoFrequency & 0xF00) + value);
                    modeTwoFreqTime = (int)(outputRate / (4194304 / (double)(32 * (2048 - modeTwoFrequency))));
                }
                else if (index == 0xFF19)
                {
                    soundRegister[index - 0xFF10] = value;

                    modeTwoFrequency = (short)((modeTwoFrequency & 0xFF) + ((value & 0x07) << 8));
                    modeTwoFreqTime = (int)(outputRate / (4194304 / (double)(32 * (2048 - modeTwoFrequency))));

                    modeTwoCounterConsecutiveSelection = (value & 0x40) == 0x40;

                    modeTwoCounterEnable = (value & 0x40) == 0x40;

                    if ((value & 0x80) == 0x80)
                    {
                        modeTwoInit = true;

                        soundOnOff |= 0x02;
                        modeTwoWatch = 0;

                        modeTwoEnvelopeState = modeTwoEnvelopeInitValue;
                    }
                }

                else if (index == 0xFF24)
                    soundOutputLevel = value;
                else if (index == 0xFF25)
                    soundOutput = value;
                // NR52
                else if (index == 0xFF26)
                    soundOnOff = value;
            }
        }

        // gets called 44100 (outPutRate) times a second
        public void UpdateBuffer()
        {
            if (currentByte >= soundBuffer.Length / 2)
                return;

            // MODE ONE:
            if (modeOneInit)
            {
                modeOneWatch++;
                modeOneEnvelopeWatch++;
                modeOneFreqWatch++;

                // end of sound
                if (modeOneWatch > modeOneSoundTime && modeOneCounterEnable)
                {
                    modeOneWatch = 0;
                    soundOnOff &= 0xFE;
                }

                // 44100/64 = 689
                if (modeOneEnvelopeWatch > 689 && modeOneEnvelopeSweeps != 0 && ((modeOneEnvelopeInc && modeOneEnvelopeState < 15) || (!modeOneEnvelopeInc && modeOneEnvelopeState > 0)))
                {
                    modeOneEnvelopeWatch = 0;

                    modeOneEnvelopeCounter++;
                    if (modeOneEnvelopeCounter >= modeOneEnvelopeSweeps)
                    {
                        modeOneEnvelopeCounter = 0;
                        modeOneEnvelopeState += modeOneEnvelopeInc ? 1 : -1;
                    }
                }

                // update wave
                if (modeOneFreqWatch > modeOneFreqTime * (modeOneFreqDuty ? modeOneWavePatternDutyPercentage : (100 - modeOneWavePatternDutyPercentage)) / 100d)
                {
                    modeOneFreqWatch = 0;

                    modeOneFreqDuty = !modeOneFreqDuty;
                    waveOne = (short)(-waveOne);
                }
            }

            // MODE TWO:
            if (modeTwoInit)
            {
                modeTwoWatch++;
                modeTwoEnvelopeWatch++;
                modeTwoFreqWatch++;

                // end of sound
                if (modeTwoWatch > modeTwoSoundTime && modeTwoCounterEnable)
                {
                    modeTwoWatch = 0;
                    soundOnOff &= 0xFE;
                }

                // 44100/64 = 689
                if (modeTwoEnvelopeWatch > 689 && modeTwoEnvelopeSweeps != 0 && ((modeTwoEnvelopeInc && modeTwoEnvelopeState < 15) || (!modeTwoEnvelopeInc && modeTwoEnvelopeState > 0)))
                {
                    modeTwoEnvelopeWatch = 0;

                    modeTwoEnvelopeCounter++;
                    if (modeTwoEnvelopeCounter >= modeTwoEnvelopeSweeps)
                    {
                        modeTwoEnvelopeCounter = 0;
                        modeTwoEnvelopeState += modeTwoEnvelopeInc ? 1 : -1;
                    }
                }

                // update wave
                if (modeTwoFreqWatch > modeTwoFreqTime * (modeTwoFreqDuty ? modeTwoWavePatternDutyPercentage : (100 - modeTwoWavePatternDutyPercentage)) / 100d)
                {
                    modeTwoFreqWatch = 0;

                    modeTwoFreqDuty = !modeTwoFreqDuty;
                    waveTwo = (short)(-waveTwo);
                }
            }

            short soundOne = (short)(waveOne * (modeOneEnvelopeState / 15d));
            short soundTwo = (short)(waveTwo * (modeTwoEnvelopeState / 15d));

            // Sound 1 ON Flag  
            if ((soundOnOff & 0x01) != 0x01)
                soundOne = 0;
            // Sound 2 ON Flag  
            if ((soundOnOff & 0x02) != 0x02)
                soundTwo = 0;

            short outPut = (short)(soundOne + soundTwo);

            if ((soundOnOff & 0x80) != 0x80)
                outPut = 0;

            soundBuffer[currentByte * 2] = (byte)(outPut & 0xFF);
            soundBuffer[currentByte * 2 + 1] = (byte)(outPut >> 8);

            currentByte++;
        }
    }
}
