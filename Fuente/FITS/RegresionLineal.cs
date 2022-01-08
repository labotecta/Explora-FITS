using System;

namespace ExploraFits
{
    class RegresionLineal
    {
        public double[,] V;         // Matriz de mínimos cuadradosa y var/covar
        public double[] C;          // Coeficientes
        public double[] SEC;        // Error estándar de los coeficientes
        public double RYSQ;         // Codeficiente de correlación múltiple
        public double SDVErr;       // Desviación estándar de los errores
        public double FReg;         // Estadístico F de Fisher de la regresión
        public double[] Ycalc;      // Y calculado
        public double[] DY;         // Residual values of Y
        public double MY;           // Media de Y
        public double MYc;          // Media de Y calculado
        public double MErr;         // Error medio
        public double[] SDVMovil;   // Desviación estándar del error móvil

        public bool Regress(double[] Y, double[,] X, double[] W, int movil)
        {
            int n_datos = Y.Length;                 // Númeero de datos
            int terminos = X.Length / n_datos;      // Terminos del polinomio = grado + 1
            int NGL = n_datos - terminos;           // Grados de libertad
            Ycalc = new double[n_datos];
            DY = new double[n_datos];
            if (NGL < 1)
            {
                // No hay datos suficientes

                return false;
            }
            V = new double[terminos, terminos];
            C = new double[terminos];
            SEC = new double[terminos];
            double[] B = new double[terminos];   // Vector for LSQ

            // Iniciar matrices

            Array.Clear(V, 0, terminos * terminos);

            // Matriz de mínimos cuadrados

            for (int i = 0; i < terminos; i++)
            {
                for (int j = 0; j < terminos; j++)
                {
                    V[i, j] = 0;
                    for (int k = 0; k < n_datos; k++)
                    {
                        V[i, j] = V[i, j] + W[k] * X[i, k] * X[j, k];
                    }
                }
                B[i] = 0;
                for (int k = 0; k < n_datos; k++)
                {
                    B[i] = B[i] + W[k] * X[i, k] * Y[k];
                }
            }

            // V ahora contiene la matriz de mínimos cuadrados sin procesar

            if (!MatrizInversa(V))
            {
                return false;
            }

            // V ahora contiene la matriz de mínimos cuadrados invertida
            // Multiplicar por B para obtener los coeficientes (C = VB)

            for (int i = 0; i < terminos; i++)
            {
                C[i] = 0;
                for (int j = 0; j < terminos; j++)
                {
                    C[i] = C[i] + V[i, j] * B[j];
                }
            }

            // Calcula estadísticas

            MY = 0;
            MYc = 0;
            MErr = 0;
            SDVMovil = new double[n_datos];
            double TSS = 0;
            double RSS = 0;
            double YBAR = 0;
            double WSUM = 0;
            for (int i = 0; i < n_datos; i++)
            {
                SDVMovil[i] = 0;
            }
            for (int k = 0; k < n_datos; k++)
            {
                YBAR += W[k] * Y[k];
                WSUM += W[k];
            }
            YBAR /= WSUM;
            for (int k = 0; k < n_datos; k++)
            {
                MY += Y[k];
                Ycalc[k] = 0;
                for (int i = 0; i < terminos; i++)
                {
                    Ycalc[k] += C[i] * X[i, k];
                }
                MYc += Ycalc[k];
                DY[k] = Ycalc[k] - Y[k];
                MErr += Math.Abs(DY[k]);
                TSS += W[k] * (Y[k] - YBAR) * (Y[k] - YBAR);
                RSS += W[k] * DY[k] * DY[k];
                if (movil > 0)
                {
                    if (k >= movil)
                    {
                        SDVMovil[k] = SDVMovil[k - 1] - W[k - movil] * DY[k - movil] * DY[k - movil] + W[k] * DY[k] * DY[k];
                    }
                    else
                    {
                        SDVMovil[movil - 1] += W[k] * DY[k] * DY[k];
                    }
                }
            }
            if (movil > 0)
            {
                for (int k = movil; k < n_datos; k++)
                {
                    SDVMovil[k] /= movil;
                    SDVMovil[k] = Math.Sqrt(SDVMovil[k]);
                }
                for (int k = 0; k < movil; k++)
                {
                    SDVMovil[k] = SDVMovil[movil];
                }
            }
            MY /= n_datos;
            MYc /= n_datos;
            MErr /= n_datos;
            double SSQ = RSS / NGL;
            RYSQ = 1 - RSS / TSS;
            FReg = 9999999;
            {
                FReg = RYSQ / (1 - RYSQ) * NGL / (terminos - 1);
            }
            SDVErr = Math.Sqrt(SSQ);

            // Calcula la matriz var-covar y el error estándar de los coeficientes

            for (int i = 0; i < terminos; i++)
            {
                for (int j = 0; j < terminos; j++)
                {
                    V[i, j] = V[i, j] * SSQ;
                }
                SEC[i] = Math.Sqrt(V[i, i]);
            }
            return true;
        }
        public bool MatrizInversa(double[,] V)
        {
            int N = (int)Math.Sqrt(V.Length);
            double[] t = new double[N];
            double[] Q = new double[N];
            double[] R = new double[N];
            double AB;
            int K, L, M;

            // Invierte la matriz simétrica V sobre ella misma

            for (M = 0; M < N; M++)
            {
                R[M] = 1;
            }
            K = 0;
            for (M = 0; M < N; M++)
            {
                double Big = 0;
                for (L = 0; L < N; L++)
                {
                    AB = Math.Abs(V[L, L]);
                    if ((AB > Big) && (R[L] != 0))
                    {
                        Big = AB;
                        K = L;
                    }
                }
                if (Big == 0)
                {
                    return false;
                }
                R[K] = 0;
                Q[K] = 1 / V[K, K];
                t[K] = 1;
                V[K, K] = 0;
                if (K != 0)
                {
                    for (L = 0; L < K; L++)
                    {
                        t[L] = V[L, K];
                        if (R[L] == 0)
                        {
                            Q[L] = V[L, K] * Q[K];
                        }
                        else
                        {
                            Q[L] = -V[L, K] * Q[K];
                        }
                        V[L, K] = 0;
                    }
                }
                if ((K + 1) < N)
                {
                    for (L = K + 1; L < N; L++)
                    {
                        if (R[L] != 0)
                        {
                            t[L] = V[K, L];
                        }
                        else
                        {
                            t[L] = -V[K, L];
                        }
                        Q[L] = -V[K, L] * Q[K];
                        V[K, L] = 0;
                    }
                }
                for (L = 0; L < N; L++)
                {
                    for (K = L; K < N; K++)
                    {
                        V[L, K] = V[L, K] + t[L] * Q[K];
                    }
                }
            }
            M = N;
            L = N - 1;
            for (K = 1; K < N; K++)
            {
                M--;
                L--;
                for (int J = 0; J <= L; J++)
                {
                    V[M, J] = V[J, M];
                }
            }
            return true;
        }
    }
}
