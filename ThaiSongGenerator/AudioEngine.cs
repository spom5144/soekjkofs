using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Wave;
using NAudio.Lame;

namespace ThaiSongGenerator
{
    public class Note
    {
        public double Frequency { get; set; }
        public double Duration { get; set; }
        public double Velocity { get; set; }
        public bool IsRest { get; set; }
    }

    public enum MusicStyle
    {
        ThaiTraditional,
        SlowSadHeartbreak,
        ThaiPop,
        ThaiBalladClear
    }

    public enum ReverbPreset
    {
        Studio, Warm, Talented, Dazzle, Distant, KTV,
        Corridor, Natural, CD, Church, Concert,
        Stereo, Theater, Phonograph, Fantasy
    }

    public enum EQPreset
    {
        Standard, DeepLows, CrispySound, ClearSound
    }

    public class AudioEngine
    {
        public const int SampleRate = 44100;
        public const int Channels = 2;
        public const int BitsPerSample = 16;

        private static readonly double[] ThaiMinorPentatonic = { 261.63, 293.66, 329.63, 392.00, 440.00 };
        private static readonly double[] ThaiSadScale = { 220.00, 246.94, 261.63, 329.63, 349.23 };
        private static readonly double[] ThaiTraditionalScale = { 261.63, 293.66, 311.13, 392.00, 415.30, 466.16, 523.25 };
        private static readonly double[] ThaiPopScale = { 261.63, 293.66, 329.63, 349.23, 392.00, 440.00, 493.88 };

        private static double[] GetScale(MusicStyle style)
        {
            return style switch
            {
                MusicStyle.ThaiTraditional => ThaiTraditionalScale,
                MusicStyle.SlowSadHeartbreak => ThaiSadScale,
                MusicStyle.ThaiPop => ThaiPopScale,
                MusicStyle.ThaiBalladClear => ThaiMinorPentatonic,
                _ => ThaiMinorPentatonic
            };
        }

        private static int GetBPM(MusicStyle style)
        {
            return style switch
            {
                MusicStyle.ThaiTraditional => 90,
                MusicStyle.SlowSadHeartbreak => 65,
                MusicStyle.ThaiPop => 120,
                MusicStyle.ThaiBalladClear => 75,
                _ => 80
            };
        }

        public static List<Note> GenerateMelody(string thaiText, MusicStyle style)
        {
            var notes = new List<Note>();
            double[] scale = GetScale(style);
            int bpm = GetBPM(style);
            double beatDuration = 60.0 / bpm;
            int prevIndex = 0;

            for (int i = 0; i < thaiText.Length; i++)
            {
                char c = thaiText[i];

                if (c == ' ')
                {
                    notes.Add(new Note { IsRest = true, Duration = beatDuration * 0.25 });
                    continue;
                }
                if (c == '\n' || c == '\r')
                {
                    notes.Add(new Note { IsRest = true, Duration = beatDuration * 0.75 });
                    continue;
                }

                if (c >= 0x0E01 && c <= 0x0E2E)
                {
                    int raw = c - 0x0E01;
                    int noteIndex = raw % scale.Length;

                    int jump = Math.Abs(noteIndex - prevIndex);
                    if (jump > 3) noteIndex = prevIndex + (noteIndex > prevIndex ? 1 : -1);
                    if (noteIndex < 0) noteIndex = 0;
                    if (noteIndex >= scale.Length) noteIndex = scale.Length - 1;

                    double freq = scale[noteIndex];
                    int octaveShift = (raw / scale.Length) % 2;
                    if (octaveShift == 1) freq *= 2.0;

                    double dur = beatDuration;
                    if (style == MusicStyle.SlowSadHeartbreak)
                        dur *= 1.2 + (raw % 3) * 0.2;

                    double vel = 0.55 + (raw % 5) * 0.08;

                    notes.Add(new Note { Frequency = freq, Duration = dur, Velocity = vel });
                    prevIndex = noteIndex;
                }
                else if (c >= 0x0E30 && c <= 0x0E3A)
                {
                    if (notes.Count > 0 && !notes[notes.Count - 1].IsRest)
                    {
                        notes[notes.Count - 1].Duration += beatDuration * 0.6;
                    }
                }
                else if (c >= 0x0E40 && c <= 0x0E44)
                {
                    if (notes.Count > 0 && !notes[notes.Count - 1].IsRest)
                    {
                        notes[notes.Count - 1].Duration += beatDuration * 0.4;
                        notes[notes.Count - 1].Velocity = Math.Min(1.0, notes[notes.Count - 1].Velocity + 0.1);
                    }
                }
                else if (c >= 0x0E48 && c <= 0x0E4B)
                {
                    if (notes.Count > 0 && !notes[notes.Count - 1].IsRest)
                    {
                        int toneLevel = c - 0x0E48;
                        double[] toneMultipliers = { 0.97, 1.03, 1.06, 0.94 };
                        notes[notes.Count - 1].Frequency *= toneMultipliers[toneLevel];
                    }
                }
                else if (c >= 0x0E50 && c <= 0x0E59)
                {
                    int digit = c - 0x0E50;
                    int noteIndex = digit % scale.Length;
                    notes.Add(new Note { Frequency = scale[noteIndex], Duration = beatDuration * 0.5, Velocity = 0.5 });
                }
            }

            if (notes.Count == 0)
            {
                foreach (char c in thaiText)
                {
                    if (char.IsWhiteSpace(c))
                    {
                        notes.Add(new Note { IsRest = true, Duration = beatDuration * 0.25 });
                    }
                    else
                    {
                        int idx = (c % scale.Length);
                        notes.Add(new Note { Frequency = scale[idx], Duration = beatDuration, Velocity = 0.6 });
                    }
                }
            }

            return notes;
        }

        public static float[] Synthesize(List<Note> melody, MusicStyle style)
        {
            double totalDuration = melody.Sum(n => n.Duration);
            int totalSamples = (int)(totalDuration * SampleRate) + SampleRate;
            float[] left = new float[totalSamples];
            float[] right = new float[totalSamples];

            int position = 0;
            foreach (var note in melody)
            {
                int noteSamples = (int)(note.Duration * SampleRate);
                if (position + noteSamples > totalSamples) noteSamples = totalSamples - position;
                if (noteSamples <= 0) break;

                if (!note.IsRest)
                {
                    for (int i = 0; i < noteSamples; i++)
                    {
                        double t = (double)i / SampleRate;
                        double env = ADSR(t, note.Duration, style);

                        double sample = GenerateWaveform(note.Frequency, t, style);
                        float val = (float)(sample * env * note.Velocity * 0.35);

                        double pan = 0.5 + 0.2 * Math.Sin(2 * Math.PI * 0.3 * t);
                        left[position + i] += val * (float)(1.0 - pan);
                        right[position + i] += val * (float)pan;
                    }
                }
                position += noteSamples;
            }

            AddBassLine(left, right, melody, style, totalSamples);
            AddPad(left, right, melody, style, totalSamples);

            float[] interleaved = new float[totalSamples * 2];
            for (int i = 0; i < totalSamples; i++)
            {
                interleaved[i * 2] = Clamp(left[i]);
                interleaved[i * 2 + 1] = Clamp(right[i]);
            }
            return interleaved;
        }

        private static double GenerateWaveform(double freq, double t, MusicStyle style)
        {
            double phase = 2 * Math.PI * freq * t;
            double sample = 0;

            switch (style)
            {
                case MusicStyle.ThaiTraditional:
                    sample = 0.6 * Math.Sin(phase) + 0.25 * Math.Sin(2 * phase) + 0.1 * Math.Sin(3 * phase) + 0.05 * Math.Sin(5 * phase);
                    break;
                case MusicStyle.SlowSadHeartbreak:
                    double vibrato = 1.0 + 0.003 * Math.Sin(2 * Math.PI * 5.5 * t);
                    phase = 2 * Math.PI * freq * vibrato * t;
                    sample = 0.7 * Math.Sin(phase) + 0.2 * Math.Sin(2 * phase) + 0.08 * Math.Sin(3 * phase);
                    double breathiness = 0.03 * (new Random((int)(t * 10000)).NextDouble() * 2 - 1);
                    sample += breathiness;
                    break;
                case MusicStyle.ThaiPop:
                    double saw = 0;
                    for (int h = 1; h <= 8; h++)
                        saw += Math.Sin(h * phase) / h;
                    sample = 0.5 * saw + 0.3 * Math.Sin(phase);
                    break;
                case MusicStyle.ThaiBalladClear:
                    sample = 0.65 * Math.Sin(phase) + 0.2 * Math.Sin(2 * phase) + 0.1 * Math.Sin(4 * phase);
                    break;
            }
            return sample;
        }

        private static double ADSR(double t, double totalDuration, MusicStyle style)
        {
            double attack, decay, sustainLevel, release;

            switch (style)
            {
                case MusicStyle.SlowSadHeartbreak:
                    attack = 0.15; decay = 0.2; sustainLevel = 0.6; release = 0.3;
                    break;
                case MusicStyle.ThaiPop:
                    attack = 0.02; decay = 0.1; sustainLevel = 0.7; release = 0.1;
                    break;
                case MusicStyle.ThaiTraditional:
                    attack = 0.05; decay = 0.15; sustainLevel = 0.65; release = 0.2;
                    break;
                default:
                    attack = 0.08; decay = 0.15; sustainLevel = 0.65; release = 0.25;
                    break;
            }

            double releaseStart = totalDuration - release;
            if (releaseStart < attack + decay) releaseStart = totalDuration * 0.7;

            if (t < attack)
                return t / attack;
            else if (t < attack + decay)
                return 1.0 - (1.0 - sustainLevel) * ((t - attack) / decay);
            else if (t < releaseStart)
                return sustainLevel;
            else if (t < totalDuration)
                return sustainLevel * (1.0 - (t - releaseStart) / (totalDuration - releaseStart));
            else
                return 0;
        }

        private static void AddBassLine(float[] left, float[] right, List<Note> melody, MusicStyle style, int totalSamples)
        {
            double bassVol = style == MusicStyle.SlowSadHeartbreak ? 0.12 : 0.15;
            int position = 0;

            foreach (var note in melody)
            {
                int noteSamples = (int)(note.Duration * SampleRate);
                if (position + noteSamples > totalSamples) noteSamples = totalSamples - position;
                if (noteSamples <= 0) break;

                if (!note.IsRest)
                {
                    double bassFreq = note.Frequency / 2.0;
                    if (bassFreq < 60) bassFreq *= 2;

                    for (int i = 0; i < noteSamples; i++)
                    {
                        double t = (double)i / SampleRate;
                        double env = ADSR(t, note.Duration, style) * 0.8;
                        double bassVal = Math.Sin(2 * Math.PI * bassFreq * t) * bassVol * env;
                        left[position + i] += (float)bassVal;
                        right[position + i] += (float)bassVal;
                    }
                }
                position += noteSamples;
            }
        }

        private static void AddPad(float[] left, float[] right, List<Note> melody, MusicStyle style, int totalSamples)
        {
            if (melody.Count == 0) return;

            double padVol = 0.06;
            double[] chordFreqs;

            var firstNote = melody.FirstOrDefault(n => !n.IsRest);
            if (firstNote == null) return;

            double root = firstNote.Frequency;
            if (style == MusicStyle.SlowSadHeartbreak)
                chordFreqs = new[] { root, root * 1.2, root * 1.5 };
            else
                chordFreqs = new[] { root, root * 1.25, root * 1.5 };

            for (int i = 0; i < totalSamples; i++)
            {
                double t = (double)i / SampleRate;
                double padSample = 0;
                foreach (var f in chordFreqs)
                    padSample += Math.Sin(2 * Math.PI * f * t) * padVol;

                double fadein = Math.Min(1.0, t / 2.0);
                double fadeout = Math.Min(1.0, (totalSamples - i) / (double)(SampleRate * 2));
                padSample *= fadein * fadeout;

                left[i] += (float)(padSample * 0.6);
                right[i] += (float)(padSample * 0.4);
            }
        }

        public static float[] ApplyReverb(float[] stereoSamples, ReverbPreset preset)
        {
            int numSamples = stereoSamples.Length / 2;
            float[] left = new float[numSamples];
            float[] right = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                left[i] = stereoSamples[i * 2];
                right[i] = stereoSamples[i * 2 + 1];
            }

            var (delays, decays, wet, predelay) = GetReverbParams(preset);

            float[] reverbL = SchroederReverb(left, delays, decays, predelay);
            float[] reverbR = SchroederReverb(right, delays.Select(d => d + 23).ToArray(), decays, predelay + 11);

            int outLen = Math.Max(reverbL.Length, reverbR.Length);
            float[] output = new float[outLen * 2];

            for (int i = 0; i < outLen; i++)
            {
                float dryL = i < numSamples ? left[i] : 0;
                float dryR = i < numSamples ? right[i] : 0;
                float wetL = i < reverbL.Length ? reverbL[i] : 0;
                float wetR = i < reverbR.Length ? reverbR[i] : 0;

                output[i * 2] = Clamp(dryL * (1 - wet) + wetL * wet);
                output[i * 2 + 1] = Clamp(dryR * (1 - wet) + wetR * wet);
            }
            return output;
        }

        private static (int[] delays, float[] decays, float wet, int predelay) GetReverbParams(ReverbPreset preset)
        {
            return preset switch
            {
                ReverbPreset.Studio => (new[] { 1557, 1617, 1491, 1422 }, new[] { 0.35f, 0.33f, 0.37f, 0.31f }, 0.25f, 200),
                ReverbPreset.Warm => (new[] { 2205, 2470, 2641, 2811 }, new[] { 0.50f, 0.48f, 0.52f, 0.46f }, 0.35f, 300),
                ReverbPreset.Talented => (new[] { 1800, 1950, 2100, 2250 }, new[] { 0.42f, 0.40f, 0.44f, 0.38f }, 0.30f, 250),
                ReverbPreset.Dazzle => (new[] { 3000, 3300, 3600, 3900 }, new[] { 0.55f, 0.53f, 0.57f, 0.51f }, 0.40f, 150),
                ReverbPreset.Distant => (new[] { 4410, 4800, 5200, 5600 }, new[] { 0.65f, 0.63f, 0.67f, 0.61f }, 0.50f, 500),
                ReverbPreset.KTV => (new[] { 2646, 2890, 3087, 3307 }, new[] { 0.55f, 0.53f, 0.57f, 0.51f }, 0.45f, 200),
                ReverbPreset.Corridor => (new[] { 3528, 3800, 4100, 4400 }, new[] { 0.50f, 0.48f, 0.52f, 0.46f }, 0.35f, 400),
                ReverbPreset.Natural => (new[] { 1764, 1940, 2100, 2250 }, new[] { 0.38f, 0.36f, 0.40f, 0.34f }, 0.22f, 100),
                ReverbPreset.CD => (new[] { 1323, 1450, 1550, 1680 }, new[] { 0.28f, 0.26f, 0.30f, 0.24f }, 0.18f, 50),
                ReverbPreset.Church => (new[] { 5292, 5700, 6100, 6500 }, new[] { 0.72f, 0.70f, 0.74f, 0.68f }, 0.55f, 600),
                ReverbPreset.Concert => (new[] { 4410, 4750, 5100, 5450 }, new[] { 0.62f, 0.60f, 0.64f, 0.58f }, 0.45f, 500),
                ReverbPreset.Stereo => (new[] { 2000, 2200, 2400, 2600 }, new[] { 0.45f, 0.43f, 0.47f, 0.41f }, 0.30f, 150),
                ReverbPreset.Theater => (new[] { 3969, 4300, 4600, 4900 }, new[] { 0.58f, 0.56f, 0.60f, 0.54f }, 0.42f, 400),
                ReverbPreset.Phonograph => (new[] { 1100, 1250, 1400, 1550 }, new[] { 0.30f, 0.28f, 0.32f, 0.26f }, 0.20f, 80),
                ReverbPreset.Fantasy => (new[] { 5500, 6000, 6500, 7000 }, new[] { 0.75f, 0.73f, 0.77f, 0.71f }, 0.60f, 700),
                _ => (new[] { 1557, 1617, 1491, 1422 }, new[] { 0.35f, 0.33f, 0.37f, 0.31f }, 0.25f, 200)
            };
        }

        private static float[] SchroederReverb(float[] input, int[] combDelays, float[] combDecays, int predelay)
        {
            int extraSamples = SampleRate * 3;
            int outLen = input.Length + extraSamples;
            float[] output = new float[outLen];

            float[] predelayed = new float[outLen];
            for (int i = 0; i < input.Length; i++)
            {
                int idx = i + predelay;
                if (idx < outLen) predelayed[idx] = input[i];
            }

            float[][] combOutputs = new float[combDelays.Length][];
            for (int c = 0; c < combDelays.Length; c++)
            {
                combOutputs[c] = CombFilter(predelayed, combDelays[c], combDecays[c]);
            }

            float[] mixed = new float[outLen];
            for (int i = 0; i < outLen; i++)
            {
                float sum = 0;
                for (int c = 0; c < combOutputs.Length; c++)
                {
                    if (i < combOutputs[c].Length) sum += combOutputs[c][i];
                }
                mixed[i] = sum / combOutputs.Length;
            }

            int[] allpassDelays = { 225, 556 };
            float allpassGain = 0.5f;
            float[] result = mixed;
            foreach (int delay in allpassDelays)
            {
                result = AllpassFilter(result, delay, allpassGain);
            }

            return result;
        }

        private static float[] CombFilter(float[] input, int delay, float decay)
        {
            float[] output = new float[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input[i];
                if (i >= delay)
                    output[i] += output[i - delay] * decay;
            }
            return output;
        }

        private static float[] AllpassFilter(float[] input, int delay, float gain)
        {
            float[] output = new float[input.Length];
            float[] buffer = new float[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                float delayed = i >= delay ? buffer[i - delay] : 0;
                buffer[i] = input[i] + gain * delayed;
                output[i] = -gain * buffer[i] + delayed;
            }
            return output;
        }

        public static float[] ApplyEQ(float[] stereoSamples, EQPreset preset)
        {
            int numSamples = stereoSamples.Length / 2;
            float[] output = new float[stereoSamples.Length];

            float[] leftIn = new float[numSamples];
            float[] rightIn = new float[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                leftIn[i] = stereoSamples[i * 2];
                rightIn[i] = stereoSamples[i * 2 + 1];
            }

            var (lowGain, midGain, highGain, lowFreq, highFreq) = GetEQParams(preset);

            float[] leftOut = ApplyThreeBandEQ(leftIn, lowGain, midGain, highGain, lowFreq, highFreq);
            float[] rightOut = ApplyThreeBandEQ(rightIn, lowGain, midGain, highGain, lowFreq, highFreq);

            for (int i = 0; i < numSamples; i++)
            {
                output[i * 2] = Clamp(leftOut[i]);
                output[i * 2 + 1] = Clamp(rightOut[i]);
            }
            return output;
        }

        private static (float lowGain, float midGain, float highGain, float lowFreq, float highFreq) GetEQParams(EQPreset preset)
        {
            return preset switch
            {
                EQPreset.Standard => (1.0f, 1.0f, 1.0f, 200f, 5000f),
                EQPreset.DeepLows => (1.8f, 0.95f, 0.85f, 250f, 4000f),
                EQPreset.CrispySound => (0.85f, 1.05f, 1.7f, 200f, 4500f),
                EQPreset.ClearSound => (0.9f, 1.4f, 1.3f, 200f, 5000f),
                _ => (1.0f, 1.0f, 1.0f, 200f, 5000f)
            };
        }

        private static float[] ApplyThreeBandEQ(float[] input, float lowGain, float midGain, float highGain, float lowFreq, float highFreq)
        {
            float[] output = new float[input.Length];
            double lowRC = 1.0 / (2.0 * Math.PI * lowFreq);
            double highRC = 1.0 / (2.0 * Math.PI * highFreq);
            double dt = 1.0 / SampleRate;
            double alphaLow = dt / (lowRC + dt);
            double alphaHigh = highRC / (highRC + dt);

            double prevLow = 0, prevHigh = 0, prevHighInput = 0;

            for (int i = 0; i < input.Length; i++)
            {
                double s = input[i];

                double low = prevLow + alphaLow * (s - prevLow);
                prevLow = low;

                double high = alphaHigh * (prevHigh + s - prevHighInput);
                prevHighInput = s;
                prevHigh = high;

                double mid = s - low - high;

                output[i] = (float)(low * lowGain + mid * midGain + high * highGain);
            }
            return output;
        }

        private static float Clamp(float value)
        {
            if (value > 0.95f) return 0.95f;
            if (value < -0.95f) return -0.95f;
            return value;
        }

        public static void Normalize(float[] samples)
        {
            float max = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                float abs = Math.Abs(samples[i]);
                if (abs > max) max = abs;
            }
            if (max > 0.01f)
            {
                float scale = 0.9f / max;
                for (int i = 0; i < samples.Length; i++)
                    samples[i] *= scale;
            }
        }

        public static byte[] ConvertToWavBytes(float[] stereoSamples)
        {
            int numSamples = stereoSamples.Length;
            int dataSize = numSamples * 2;
            int fileSize = 44 + dataSize;

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            bw.Write(fileSize - 8);
            bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            bw.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)Channels);
            bw.Write(SampleRate);
            bw.Write(SampleRate * Channels * 2);
            bw.Write((short)(Channels * 2));
            bw.Write((short)BitsPerSample);
            bw.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            bw.Write(dataSize);

            for (int i = 0; i < numSamples; i++)
            {
                short val = (short)(stereoSamples[i] * 32767);
                bw.Write(val);
            }

            return ms.ToArray();
        }

        public static void ExportToMp3(float[] stereoSamples, string outputPath, int bitrate = 192)
        {
            byte[] wavBytes = ConvertToWavBytes(stereoSamples);

            using var wavStream = new MemoryStream(wavBytes);
            using var reader = new WaveFileReader(wavStream);
            using var writer = new LameMP3FileWriter(outputPath, reader.WaveFormat, bitrate);
            reader.CopyTo(writer);
        }

        public static void ExportToWav(float[] stereoSamples, string outputPath)
        {
            byte[] wavBytes = ConvertToWavBytes(stereoSamples);
            File.WriteAllBytes(outputPath, wavBytes);
        }

        public static float[] GenerateSong(string lyrics, MusicStyle style, ReverbPreset reverb, EQPreset eq, Action<int>? progressCallback = null)
        {
            progressCallback?.Invoke(10);
            var melody = GenerateMelody(lyrics, style);

            progressCallback?.Invoke(30);
            float[] audio = Synthesize(melody, style);

            progressCallback?.Invoke(50);
            audio = ApplyEQ(audio, eq);

            progressCallback?.Invoke(70);
            audio = ApplyReverb(audio, reverb);

            progressCallback?.Invoke(90);
            Normalize(audio);

            progressCallback?.Invoke(100);
            return audio;
        }
    }
}
