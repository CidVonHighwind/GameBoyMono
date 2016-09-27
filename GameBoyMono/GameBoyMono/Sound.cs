using Microsoft.Xna.Framework;
using NAudio.Wave;
using System;

namespace GameBoyMono
{
    public class Sound
    {
        BufferedWaveProvider bufferProvider;
        WaveOut waveOut;

        const int outputRate = 44100;

        public byte[] soundBuffer = new byte[1470];
        int currentByte;

        // FF10-FF3F
        byte[] soundRegister = new byte[0x30];

        // frame sequencer
        int frameSequencerTimer;
        byte frameSequencerCounter;

        // MODE ONE
        byte modeOneNumberofSweepShifts;
        bool modeOneSweepSubtraction;
        byte modeOneSweepTime;
        byte modeOneSweepCounter;

        float modeOneWavePatternDutyPercentage;

        byte modeOneEnvelopeSteps;
        bool modeOneEnvelopeInc;
        int modeOneEnvelopeCounter;
        int modeOneEnvelopeState;

        byte modeOneLenghtCounter;
        int modeOneSoundLenght;

        short modeOneFrequency;
        int modeOneFreqWatch;
        int modeOneFreqTime;
        bool modeOneFreqDuty;

        bool modeOneCounterEnable;

        bool modeOneInit;
        short waveOne = 5000;


        // MODE TWO
        float modeTwoWavePatternDutyPercentage;

        byte modeTwoEnvelopeSteps;
        bool modeTwoEnvelopeInc;
        int modeTwoEnvelopeCounter;
        int modeTwoEnvelopeState;

        byte modeTwoLenghtCounter;
        int modeTwoSoundLenght;

        short modeTwoFrequency;
        int modeTwoFreqWatch;
        int modeTwoFreqTime;
        bool modeTwoFreqDuty;

        bool modeTwoCounterEnable;

        bool modeTwoInit;
        short waveTwo = 5000;


        // MODE THREE
        bool modeThreeOn;

        byte modeThreeLenghtCounter;
        int modeThreeSoundLenght;

        byte modeThreeOutputLevel;

        short modeThreeFrequency;
        int modeThreeFreqTime;
        int modeThreeWatch;

        bool modeThreeInit;
        bool modeThreeCounterEnable;

        byte[] waveNibbles = new byte[32];

        short modeThreeMaxVolume = 5000;
        short waveThree;

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
                if (index == 0xFF10)
                    return soundRegister[index - 0xFF10];
                else if (index == 0xFF11)
                    return (byte)(soundRegister[index - 0xFF10] & 0xC0);
                else if (index == 0xFF12)
                    return soundRegister[index - 0xFF10];
                else if (index == 0xFF13)
                    return 0;
                else if (index == 0xFF14)
                    return soundRegister[index - 0xFF10];

                // MODE TWO
                else if (index == 0xFF16)
                    return (byte)(soundRegister[index - 0xFF10] & 0xC0);
                else if (index == 0xFF17)
                    return soundRegister[index - 0xFF10];
                else if (index == 0xFF18)
                    return 0;
                else if (index == 0xFF19)
                    return soundRegister[index - 0xFF10];

                // Stuff
                else if (index == 0xFF24)
                    return soundOutputLevel;
                else if (index == 0xFF25)
                    return soundOutput;
                else if (index == 0xFF26)
                    return soundOnOff;

                else if (index == 0xFF1A)
                    return (byte)(modeThreeOn ? 0x80 : 0x00);
                else if (index == 0xFF1B)
                    return 0;
                else if (index == 0xFF1C)
                    return (byte)(modeThreeOutputLevel << 5);
                else if (index == 0xFF1D)
                    return 0;
                else if (index == 0xFF1E)
                    return (byte)(modeThreeCounterEnable ? 0x40 : 0x00);


                return soundRegister[index - 0xFF10];
            }
            set
            {

                if (index == 0xFF10)
                {
                    soundRegister[index - 0xFF10] = (byte)(value & 0x7F);

                    modeOneNumberofSweepShifts = (byte)(value & 0x07);
                    modeOneSweepSubtraction = (value & 0x08) == 0x08;
                    modeOneSweepTime = (byte)((value & 0x70) >> 4);

                    modeOneSweepCounter = 0;
                }
                else if (index == 0xFF11)
                {
                    soundRegister[index - 0xFF10] = (byte)(value & 0xC7);

                    // Sound Length = (64-t1) * (1/256) sec
                    modeOneSoundLenght = 64 - (byte)(value & 0x07);

                    // 12.5, 25, 50, 75
                    byte waveDuty = (byte)(value >> 6);
                    modeOneWavePatternDutyPercentage = 25 * waveDuty + ((waveDuty == 0) ? 12.5f : 0);
                }
                else if (index == 0xFF12)
                {
                    soundRegister[index - 0xFF10] = value;

                    modeOneEnvelopeSteps = (byte)(value & 0x07);
                    modeOneEnvelopeInc = (value & 0x08) == 0x08;
                }
                else if (index == 0xFF13)
                {
                    soundRegister[index - 0xFF10] = value;

                    modeOneFrequency = (short)((modeOneFrequency & 0xF00) + value);
                    modeOneFreqTime = (int)(outputRate / (4194304 / (double)(32 * (2048 - modeOneFrequency))));
                }
                else if (index == 0xFF14)
                {
                    soundRegister[index - 0xFF10] = (byte)(value & 0x40);

                    modeOneFrequency = (short)((modeOneFrequency & 0xFF) + ((value & 0x07) << 8));
                    modeOneFreqTime = (int)(outputRate / (4194304 / (double)(32 * (2048 - modeOneFrequency))));

                    modeOneCounterEnable = (value & 0x40) == 0x40;

                    if ((value & 0x80) == 0x80)
                    {
                        modeOneInit = true;

                        soundOnOff |= 0x01;

                        modeOneLenghtCounter = 0;

                        // set to initial value
                        modeOneEnvelopeState = (byte)(this[0xFF12] >> 4);
                    }
                }
                // not used
                else if (index == 0xFF15) { }

                // MODE TWO
                else if (index == 0xFF16)
                {
                    soundRegister[index - 0xFF10] = (byte)(value & 0xC7);

                    // Sound Length = (64-t1) * (1/256) sec
                    modeTwoSoundLenght = 64 - (byte)(value & 0x07);

                    // 12.5, 25, 50, 75
                    byte waveDuty = (byte)(value >> 6);
                    modeTwoWavePatternDutyPercentage = 25 * waveDuty + ((waveDuty == 0) ? 12.5f : 0);
                }
                else if (index == 0xFF17)
                {
                    soundRegister[index - 0xFF10] = value;

                    modeTwoEnvelopeSteps = (byte)(value & 0x07);
                    modeTwoEnvelopeInc = (value & 0x08) == 0x08;
                }
                else if (index == 0xFF18)
                {
                    soundRegister[index - 0xFF10] = value;

                    modeTwoFrequency = (short)((modeTwoFrequency & 0xF00) + value);
                    modeTwoFreqTime = (int)(outputRate / (4194304 / (double)(32 * (2048 - modeTwoFrequency))));
                }
                else if (index == 0xFF19)
                {
                    soundRegister[index - 0xFF10] = (byte)(value & 0x40);

                    modeTwoFrequency = (short)((modeTwoFrequency & 0xFF) + ((value & 0x07) << 8));
                    modeTwoFreqTime = (int)(outputRate / (4194304 / (double)(32 * (2048 - modeTwoFrequency))));

                    modeTwoCounterEnable = (value & 0x40) == 0x40;

                    if ((value & 0x80) == 0x80)
                    {
                        modeTwoInit = true;

                        soundOnOff |= 0x02;

                        modeTwoLenghtCounter = 0;

                        // set to initial value
                        modeTwoEnvelopeState = (byte)(this[0xFF17] >> 4);
                    }
                }

                // MODE THREE
                else if (index == 0xFF1A)
                {
                    modeThreeOn = (value & 0x80) == 0x80;
                }
                else if (index == 0xFF1B)
                {
                    // (256-t1) * (1/256) sec
                    modeThreeSoundLenght = 256 - value;
                }
                else if (index == 0xFF1C)
                {
                    modeThreeOutputLevel = (byte)((value >> 5) & 0x03);

                    if(modeThreeOutputLevel == 0x00)
                    {
                        modeThreeWatch = 0;
                    }
                }
                else if (index == 0xFF1D)
                {
                    modeThreeFrequency = (short)((modeThreeFrequency & 0xF00) + value);
                    modeThreeFreqTime = (int)(outputRate / (4194304 / (double)(32 * (2048 - modeThreeFrequency))));

                    modeThreeWatch = 0;
                }
                else if (index == 0xFF1E)
                {
                    modeThreeFrequency = (short)((modeThreeFrequency & 0xFF) + ((value & 0x07) << 8));
                    modeThreeFreqTime = (int)(outputRate / (4194304 / (double)(32 * (2048 - modeThreeFrequency))));

                    modeThreeCounterEnable = (value & 0x40) == 0x40;
                    modeThreeInit = (value & 0x80) == 0x80;

                    // start sound 3 again
                    if (modeThreeOn && modeThreeInit)
                    {
                        soundOnOff |= 0x04;

                        modeThreeLenghtCounter = 0;

                        modeThreeWatch = 0;
                    }
                    else
                    {
                        soundOnOff = (byte)(soundOnOff & 0xFB);
                    }
                }


                else if (index == 0xFF24)
                    soundOutputLevel = value;
                else if (index == 0xFF25)
                    soundOutput = value;
                // NR52
                else if (index == 0xFF26)
                    soundOnOff = value;

                // wave data
                else if(0xFF30 <= index && index <= 0xFF3F)
                {
                    soundRegister[index - 0xFF10] = value;

                    waveNibbles[index - 0xFF30] = (byte)(value >> 4);
                    waveNibbles[index - 0xFF30 + 1] = (byte)(value & 0x0F);
                }
            }
        }

        // gets called 44100 (outPutRate) times a second
        public void UpdateBuffer()
        {
            if (currentByte >= soundBuffer.Length / 2)
                return;

            frameSequencerTimer++;

            // 44100/512 = 86
            if (frameSequencerTimer >= 86)
            {
                frameSequencerTimer = 0;

                // 256Hz Length Ctr Clock
                if (frameSequencerCounter % 2 == 0)
                {
                    modeOneLenghtCounter++;
                    modeTwoLenghtCounter++;
                    modeThreeLenghtCounter++;

                    // deactivate channel 1
                    if (modeOneLenghtCounter >= modeOneSoundLenght && modeOneCounterEnable)
                        soundOnOff &= 0xFE;

                    // deactivate channel 2
                    if (modeTwoLenghtCounter >= modeTwoSoundLenght && modeTwoCounterEnable)
                        soundOnOff &= 0xFD;

                    // deactivate channel 3
                    if (modeThreeLenghtCounter >= modeThreeSoundLenght && modeThreeCounterEnable)
                    {
                        modeThreeInit = false;
                        soundOnOff &= 0xFB;
                    }
                }

                // 64Hz Volume Envelope Clock
                if (frameSequencerCounter % 7 == 0)
                {
                    modeOneEnvelopeCounter++;
                    modeTwoEnvelopeCounter++;

                    // step channel 1 up/down
                    if (modeOneEnvelopeSteps != 0 && modeOneEnvelopeCounter >= modeOneEnvelopeSteps &&
                      ((modeOneEnvelopeInc && modeOneEnvelopeState < 15) || (!modeOneEnvelopeInc && modeOneEnvelopeState > 0)))
                    {
                        modeOneEnvelopeCounter = 0;
                        modeOneEnvelopeState += modeOneEnvelopeInc ? 1 : -1;
                    }

                    // step channel 2 up/down
                    if (modeTwoEnvelopeSteps != 0 && modeTwoEnvelopeCounter >= modeTwoEnvelopeSteps &&
                      ((modeTwoEnvelopeInc && modeTwoEnvelopeState < 15) || (!modeTwoEnvelopeInc && modeTwoEnvelopeState > 0)))
                    {
                        modeTwoEnvelopeCounter = 0;
                        modeTwoEnvelopeState += modeTwoEnvelopeInc ? 1 : -1;
                    }
                }

                // 128Hz Sweep Clock
                if((frameSequencerCounter + 2) % 4 == 0)
                {
                    modeOneSweepCounter++;

                    // sweep
                    if(modeOneSweepTime != 0 && modeOneNumberofSweepShifts != 0 && modeOneSweepCounter >= modeOneSweepTime)
                    {
                        modeOneSweepCounter = 0;
                        short newFreq = (short)(modeOneFrequency + (modeOneSweepSubtraction ? -1 : 1) * (modeOneFrequency >> modeOneNumberofSweepShifts));

                        // newFreq > 11bits
                        if ((newFreq & 0x7FF) != newFreq)
                        {
                            // deactivate channel
                            soundOnOff &= 0xFE;
                        }
                        else if(newFreq <= 0)
                        {

                        }
                        else
                        {
                            modeOneFrequency = newFreq;
                        }

                        soundRegister[0xFF13 - 0xFF10] = (byte)(modeOneFrequency & 0xFF);
                        soundRegister[0xFF14 - 0xFF10] = (byte)(soundRegister[0xFF14 - 0xFF10] & 0xF8 + (modeOneFrequency >> 8));

                        modeOneFreqTime = (int)(outputRate / (4194304 / (double)(32 * (2048 - modeOneFrequency))));
                    }
                }

                frameSequencerCounter++;
                if (frameSequencerCounter > 7)
                    frameSequencerCounter = 0;
            }

            // MODE ONE:
            if (modeOneInit)
            {
                modeOneFreqWatch++;

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
                modeTwoFreqWatch++;

                // update wave
                if (modeTwoFreqWatch > modeTwoFreqTime * (modeTwoFreqDuty ? modeTwoWavePatternDutyPercentage : (100 - modeTwoWavePatternDutyPercentage)) / 100d)
                {
                    modeTwoFreqWatch = 0;

                    modeTwoFreqDuty = !modeTwoFreqDuty;
                    waveTwo = (short)(-waveTwo);
                }
            }

            // MODE THREE:
            if (modeThreeInit)
            {
                byte currentNibble = (byte)((modeThreeWatch / (double)modeThreeFreqTime) * 32);
                byte wave = (byte)(waveNibbles[currentNibble] >> (modeThreeOutputLevel - 1));
                waveThree = (short)(modeThreeMaxVolume * (wave / 15d));

                modeThreeWatch++;

                if (modeThreeWatch >= modeThreeFreqTime)
                {
                    modeThreeWatch = 0;

                    modeThreeMaxVolume = (short)(-modeThreeMaxVolume);
                }

            }

            short soundOne = (short)(waveOne * (modeOneEnvelopeState / 15d));
            short soundTwo = (short)(waveTwo * (modeTwoEnvelopeState / 15d));
            short soundThree = waveThree;

            // Sound 1 ON Flag  
            if ((soundOnOff & 0x01) == 0x00)
                soundOne = 0;
            // Sound 2 ON Flag  
            if ((soundOnOff & 0x02) == 0x00)
                soundTwo = 0;
            // Sound 3 ON Flag  
            if ((soundOnOff & 0x04) == 0x00 || modeThreeOutputLevel == 0x00 || !modeThreeOn)
                soundThree = 0;

            short outPut = (short)((soundOne + soundTwo + soundThree) * 0.05f);

            if ((soundOnOff & 0x80) != 0x80)
                outPut = 0;

            soundBuffer[currentByte * 2] = (byte)(outPut & 0xFF);
            soundBuffer[currentByte * 2 + 1] = (byte)(outPut >> 8);

            currentByte++;
        }
    }
}
