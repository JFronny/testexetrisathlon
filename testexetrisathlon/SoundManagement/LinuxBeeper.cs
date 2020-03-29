using System;
using System.Collections.Generic;
using System.IO;
using Bassoon;

namespace testexetrisathlon.SoundManagement
{
    public class Beeper : IBeeper
    {
        private static readonly Dictionary<Tuple<int, int>, Tuple<string, Sound>> Beeps = new Dictionary<Tuple<int, int>, Tuple<string, Sound>>();
        public void Beep(int frequency, int duration)
        {
            Tuple<int, int> key = new Tuple<int, int>(frequency, duration);
            if (!Beeps.ContainsKey(key))
            {
                string file = Path.GetTempFileName();
                File.Move(file, Path.ChangeExtension(file, "wav"));
                file = Path.ChangeExtension(file, "wav");
                File.WriteAllBytes(file, BeepBeep(1000, frequency, duration));
                Beeps.Add(key, new Tuple<string, Sound>(file, new Sound(file)));
            }
            Beeps[key].Item2.Cursor = 0;
            Beeps[key].Item2.Play();
            Console.Clear();
        }
        
        private static byte[] BeepBeep(int amplitude, int frequency, int duration)
        {
            double a = ((amplitude * Math.Pow(2, 15)) / 1000) - 1;
            double deltaFt = (2 * Math.PI * frequency) / 44100.0;

            int samples = (441 * duration) / 10;
            int bytes = samples * 4;
            int[] hdr = {0X46464952, 36 + bytes, 0X45564157, 0X20746D66, 16, 0X20001, 44100, 176400, 0X100004, 0X61746164, bytes};
            using MemoryStream ms = new MemoryStream(44 + bytes);
            using BinaryWriter bw = new BinaryWriter(ms);
            for (int I = 0; I < hdr.Length; I++) bw.Write(hdr[I]);
            for (int T = 0; T < samples; T++)
            {
                short sample = Convert.ToInt16(a * Math.Sin(deltaFt * T));
                bw.Write(sample);
                bw.Write(sample);
            }
            bw.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        public void Dispose()
        {
            foreach (Tuple<string, Sound> file in Beeps.Values) File.Delete(file.Item1);
        }
    }
}