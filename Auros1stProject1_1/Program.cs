using System;
using static System.Math;
using static System.Console;
using System.Collections.Generic;
using System.IO;

//
// 과제 1-1.
// Psi와 Delta 스펙트럼을 읽어 들여서, alpha와 beta로 변환한다.
// "SiO2 2nm_on_Si.dat" 파일의 데이터를 변환 -> "SiO2 2nm_on_Si_new.dat" 파일로 저장.
// 

namespace Auros1stProject1_1
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            // "SiO2 2nm_on_Si.dat" 로딩. (tsv 형식)
            // wavelength : 350 ~ 980(nm) 이내의 데이터를 저장한다.
            //
            // 컬럼: wavelength(nm), AOI, Psi, Delta
            //
            // 2021.03.22 이지원.
            //
            #region "SiO2 2nm_on_Si.dat" 로딩.


            List<string> MeasurementSpectrumData = new List<string>();  // 측정 스펙트럼 데이터 저장할 배열. (한 줄씩 저장)
            string[] SingleLineData;                                    // 한 줄의 스펙트럼 데이터를 임시로 저장할 배열.
            
            // "SiO2 2nm_on_Si.dat" 파일 읽기. (한 줄씩)
            MeasurementSpectrumData.AddRange(File.ReadAllLines("SiO2 2nm_on_Si.dat"));

            // 무의미한 공백 행을 제거한다.
            int lenSpectrumData = MeasurementSpectrumData.Count;
            string Blank = "";
            for (int i = 0; i < lenSpectrumData; i++)
            {
                if (MeasurementSpectrumData[0] == Blank)
                    MeasurementSpectrumData.RemoveAt(0);
                else
                    break;
            }

            // wavelength : 350 ~ 980(nm)인 측정 스펙트럼 데이터를 담을 리스트 선언.
            List<double> wavelength = new List<double>();   // 파장 데이터 리스트.
            List<double> AOI = new List<double>();          // 입사각 데이터 리스트.
            List<double> psi_alpha = new List<double>();    // Psi 데이터 리스트.
            List<double> delta_beta = new List<double>();   // Delta 데이터 리스트.

            // 데이터의 첫번째 줄은 column 명이다.
            // 이를 제외하기 위해 반복문을 1부터 시작한다.
            int StartIndex = 1;
            int LoopNum = MeasurementSpectrumData.Count;
            for (int i = StartIndex; i < LoopNum; i++)
            {
                // tsv 형식의 데이터를 SingleLineData에 저장한다.
                SingleLineData = MeasurementSpectrumData[i].Split((char)0x09);  // 0x09 : 수평 탭.
                // 파장이 350 ~ 980(nm) 이내인 데이터만 저장한다.
                if (Convert.ToDouble(SingleLineData[0]) >= 350.0 &&
                    Convert.ToDouble(SingleLineData[0]) <= 980.0)
                {
                    // 각 컬럼에 해당하는 데이터를 저장한다.
                    wavelength.Add(Double.Parse(SingleLineData[0]));
                    AOI.Add(Double.Parse(SingleLineData[1]));
                    psi_alpha.Add(Double.Parse(SingleLineData[2]));
                    delta_beta.Add(Double.Parse(SingleLineData[3]));
                }
            }

            #region 저장된 스펙트럼 데이터 출력문 (Test)
            /*foreach (var item in wavelength)
                WriteLine(item);
            WriteLine("==========");
            foreach (var item in AOI)
                WriteLine(item);
            WriteLine("==========");
            foreach (var item in psi_alpha)
                WriteLine(item);
            WriteLine("==========");
            foreach (var item in delta_beta)
                WriteLine(item);
            WriteLine("==========");*/
            #endregion
            #endregion

            //
            // 위에서 저장된 스펙트럼 데이터의
            // psi, delta 값을 alpha, beta 로 변환한다.
            // 
            // 2021.03.22 이지원.
            //
            #region psi, delta -> alpha, beta 변환.

            // degree 를 radian 으로 변환해주는 함수.
            double degree2radian(double angle) => ((angle * (PI)) / 180);

            // Polarizer offset 각도. (45도)
            double PolarizerRadian = degree2radian(45.0);

            // psi, delta 데이터를 alpha, beta 로 변환한다.
            LoopNum = wavelength.Count;
            for (int i = 0; i < LoopNum; i++)
            {
                // psi, delta 값을 radian 으로 변환한다.
                double PsiRadian = degree2radian(psi_alpha[i]);
                double DeltaRadian = degree2radian(delta_beta[i]);

                // psi, delta 데이터를 alpha, beta 로 갱신한다.
                psi_alpha[i] = (
                    (Pow(Tan(PsiRadian), 2.0) - Pow(Tan(PolarizerRadian), 2.0))
                    / (Pow(Tan(PsiRadian), 2.0) + Pow(Tan(PolarizerRadian), 2.0)));
                delta_beta[i] = (
                    (2 * Tan(PsiRadian) * Tan(PolarizerRadian) * Cos(DeltaRadian))
                    / (Pow(Tan(PsiRadian), 2.0) + Pow(Tan(PolarizerRadian), 2.0)));
            }

            #region 갱신된 스펙트럼 데이터 출력문 (Test)
            /*foreach (var item in wavelength)
                WriteLine(item);
            WriteLine("==========");
            foreach (var item in AOI)
                WriteLine(item);
            WriteLine("==========");
            foreach (var item in psi_alpha)
                WriteLine(item);
            WriteLine("==========");
            foreach (var item in delta_beta)
                WriteLine(item);
            WriteLine("==========");*/
            #endregion
            #endregion

            //
            // 변환된 스펙트럼 데이터를 “SiO2 2nm_on_Si_new.dat” 파일로 저장한다.
            //
            // 컬럼 : wavelength(nm), AOI, alpha, beta
            //
            // 2021.03.22 이지원.
            //
            #region 변환된 스펙트럼 데이터를 "SiO2 2nm_on_Si_new.dat" 파일로 저장.

            // 파일 쓰기.
            using (StreamWriter NewSpectrumOutputFile = new StreamWriter("SiO2 2nm_on_Si_new.dat"))
            {
                // 컬럼 명 쓰기.
                NewSpectrumOutputFile.WriteLine(
                    "wavelength(nm)" + "\t"
                    + "AOI" + "\t"
                    + "alpha" + "\t"
                    + "beta");    // 컬럼명 쓰기.
                // WriteLine(Columns);

                // 스펙트럼 데이터 쓰기.
                for (int i = 0; i < LoopNum; i++)
                {
                    // tsv 데이터 형식으로 데이터를 쓴다.
                    NewSpectrumOutputFile.WriteLine(
                        wavelength[i] + "\t"
                        + AOI[i] + "\t"
                        + psi_alpha[i] + "\t"
                        + delta_beta[i]);
                }
            }

            #endregion
        }
    }
}
