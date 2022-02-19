using System;

namespace ExploraFits
{
    class RegresionLineal
    {
        public double[,] V;         // Matriz de mínimos cuadradosa y var/covar
        public double[] CA;         // Coeficientes
        public double[] CB;         // Coeficientes
        //private double[] SEC;     // Error estándar de los coeficientes
        public double RYSQa;        // Codeficiente de correlación múltiple
        public double RYSQb;
        public double SDVErra;      // Desviación estándar de los errores
        public double SDVErrb;      // Desviación estándar de los errores
        public double FRega;        // Estadístico F de Fisher de la regresión
        public double FRegb;        // Estadístico F de Fisher de la regresión
        public double[] Ycalc;      // Y calculado
        private double[] DY;        // Residual values of Y
        public double MY;           // Media de Y
        public double MYc;          // Media de Y calculado
        public double MErr;         // Error medio
        public double[] SDVMovil;   // Desviación estándar del error móvil

        public bool Regress(double[] Y, double[,] X, double[] W, int distancia_movil)
        {
            int n_datos = Y.Length;                         // Número de datos
            int terminos_polinomio = X.Length / n_datos;    // Terminos del polinomio = grado + 1
            int NGL = n_datos - terminos_polinomio;         // Grados de libertad
            Ycalc = new double[n_datos];
            if (NGL < 1)
            {
                // No hay datos suficientes

                return false;
            }
            V = new double[terminos_polinomio, terminos_polinomio];
            CA = new double[terminos_polinomio];
            //SEC = new double[terminos];
            double[] B = new double[terminos_polinomio];   // Vector for LSQ

            // Iniciar matrices

            Array.Clear(V, 0, terminos_polinomio * terminos_polinomio);

            // Matriz de mínimos cuadrados

            for (int i = 0; i < terminos_polinomio; i++)
            {
                for (int j = 0; j < terminos_polinomio; j++)
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

            for (int i = 0; i < terminos_polinomio; i++)
            {
                CA[i] = 0;
                for (int j = 0; j < terminos_polinomio; j++)
                {
                    CA[i] = CA[i] + V[i, j] * B[j];
                }
            }
            for (int k = 0; k < n_datos; k++)
            {
                Ycalc[k] = 0;
                for (int i = 0; i < terminos_polinomio; i++)
                {
                    Ycalc[k] += CA[i] * X[i, k];
                }
            }
            Estadisticas(Y, X, W, distancia_movil);
            return true;
        }
        public bool Estadisticas(double[] Y, double[,] X, double[] W, int distancia_movil)
        {
            // Calcula estadísticas

            int n_datos = Y.Length;
            int terminos = X.Length / n_datos;
            int NGL = n_datos - terminos;
            DY = new double[n_datos];
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
                MYc += Ycalc[k];
                DY[k] = Ycalc[k] - Y[k];
                MErr += Math.Abs(DY[k]);
                TSS += W[k] * (Y[k] - YBAR) * (Y[k] - YBAR);
                RSS += W[k] * DY[k] * DY[k];
                if (distancia_movil > 0)
                {
                    if (k >= distancia_movil)
                    {
                        SDVMovil[k] = SDVMovil[k - 1] - W[k - distancia_movil] * DY[k - distancia_movil] * DY[k - distancia_movil] + W[k] * DY[k] * DY[k];
                    }
                    else
                    {
                        SDVMovil[distancia_movil - 1] += W[k] * DY[k] * DY[k];
                    }
                }
            }
            if (distancia_movil > 0)
            {
                for (int k = distancia_movil; k < n_datos; k++)
                {
                    SDVMovil[k] /= distancia_movil;
                    SDVMovil[k] = Math.Sqrt(SDVMovil[k]);
                }
                for (int k = 0; k < distancia_movil; k++)
                {
                    SDVMovil[k] = SDVMovil[distancia_movil];
                }
            }
            MY /= n_datos;
            MYc /= n_datos;
            MErr /= n_datos;
            double SSQ = RSS / NGL;
            RYSQa = RYSQb = 1 - RSS / TSS;
            FRega = 9999999;
            {
                FRega = RYSQa / (1 - RYSQa) * NGL / (terminos - 1);
            }
            FRegb = FRega;
            SDVErra = SDVErrb = Math.Sqrt(SSQ);

            // Calcula la matriz var-covar y el error estándar de los coeficientes

            /*for (int i = 0; i < terminos; i++)
            {
                for (int j = 0; j < terminos; j++)
                {
                    V[i, j] = V[i, j] * SSQ;
                }
                SEC[i] = Math.Sqrt(V[i, i]);
            }*/
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
