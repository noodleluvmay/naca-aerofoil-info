using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NACA_5_digit_NL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // for plot initialise center position of plotting space
            center_x = panel_plot.Width / 2;
            center_y = panel_plot.Height / 2;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // set initial aerofoil
            aerofoil_tb.Text = "23012";
            alpha_tb.Text = "4";

            // extract info from digits
            double designCL = Convert.ToDouble(aerofoil_tb.Text.Substring(0, 1)) * 3 / 20; // designed CL, calculated from 1st digit * 3/20
            double location = Convert.ToDouble(aerofoil_tb.Text.Substring(1, 1)) / 20; // max camber location, 2nd digit / 20
            double maxThickness = Convert.ToDouble(aerofoil_tb.Text.Substring(3, 2))/100; //max thickness, last 2 digits

            if (aerofoil_tb.Text[2] == '0') // if 3rd digit is 0, normal aerofoil
            {
                camber_tb.Text = "Normal";
            }
            else if (aerofoil_tb.Text[2] == '1') // if 3rd digit is 1, reflex aerofoil
            {
                camber_tb.Text = "Reflex";
            }

            // display extracted info to text box
            designCL_tb.Text = designCL.ToString();
            location_tb.Text = location.ToString();
            maxThickness_tb.Text = maxThickness.ToString();
            m_tb.Text = Convert.ToString(MValue(location, aerofoil_tb.Text[2]));
            k_tb.Text = Convert.ToString(KValue(location, aerofoil_tb.Text[2]));

            k2k1_tb.Text = "N/A"; // initial standard 5 digit, K2/K1 not applicable

            double limit = Math.Acos(1 - 2 * location / 10); //cartesian to polar
            //intlimtb.Text = Convert.ToString(limit);
        }

        static double MValue(double location, char camber) // find m value according to location and camber
        {
            var mValueList = new List<double>(); // declare empty list

            if (camber == '0') // normal camber
            {
                var mValueList0 = new List<double>() { 0.058, 0.126, 0.2025, 0.29, 0.391 };
                mValueList.AddRange(mValueList0); //add new list to initial empty list
            }

            else if (camber == '1') // reflex camber
            {
                var mValueList1 = new List<double>() { 0, 0.13, 0.217, 0.318, 0.441 };
                mValueList.AddRange(mValueList1); //add new list to initial empty list
            }

            double rValue = mValueList[Convert.ToInt32(location * 20) - 1]; //find m based on index

            return rValue;
        }

        static double KValue(double location, char camber) // find k1 value according to location and camber
        {
            var kValueList = new List<double>(); // declare empty list

            if (camber == '0') // normal camber
            {
                var kValueList0 = new List<double>() { 361.4, 51.64, 15.957, 6.643, 3.23 };
                kValueList.AddRange(kValueList0); //add new list to initial empty list
            }

            else if (camber == '1') // reflex camber
            {
                var kValueList1 = new List<double>() { 0, 51.99, 15.793, 6.52, 3.191 }; // 0 is dummy as reflex starts 10%
                kValueList.AddRange(kValueList1); //add new list to initial empty list
            }

            double rValue = kValueList[Convert.ToInt32(location * 20) - 1]; //find m based on index

            return rValue;
        }

        static double K2K1Value(double location, char camber) // find k2/k1 value according to location and camber for reflex
        {
            var kValueList = new List<double>() { 0, 0.000764, 0.00677, 0.0303, 0.1355 }; // declare list of k2/k1 value, 0 is dummy
            double rValue = kValueList[Convert.ToInt32(location * 20) - 1]; //find m based on index

            return rValue;
        }

        private void aerofoilData_bt_Click(object sender, EventArgs e)
        {
            if (aerofoil_tb.Text.Length == 5) // for 5 digit
            {
                double designCL = Convert.ToDouble(aerofoil_tb.Text.Substring(0, 1)) * 3 / 20; // designed CL, calculated from 1st digit * 3/20
                double location = Convert.ToDouble(aerofoil_tb.Text.Substring(1, 1)) / 20; // max camber location, 2nd digit / 20
                double maxThickness = Convert.ToDouble(aerofoil_tb.Text.Substring(3, 2)) / 100; //max thickness, last 2 digits

                if (aerofoil_tb.Text[2] == '0') // if 3rd digit is 0, normal aerofoil
                {
                    camber_tb.Text = "Normal";
                }
                else if (aerofoil_tb.Text[2] == '1') // if 3rd digit is 1, reflex aerofoil
                {
                    camber_tb.Text = "Reflex";
                }

                designCL_tb.Text = designCL.ToString();
                location_tb.Text = location.ToString();
                maxThickness_tb.Text = maxThickness.ToString();
                m_tb.Text = Convert.ToString(MValue(location, aerofoil_tb.Text[2]));
                k_tb.Text = Convert.ToString(KValue(location, aerofoil_tb.Text[2]));

                if (aerofoil_tb.Text[2] == '0') // if normal aerofoil, k2/k1 value N/A
                {
                    k2k1_tb.Text = "N/A";
                }
                else if (aerofoil_tb.Text[2] == '1') // if reflex aerofoil, k2/k1 determined from method
                {
                    k2k1_tb.Text = Convert.ToString(K2K1Value(location, aerofoil_tb.Text[2]));
                }
            }
            else if (aerofoil_tb.Text.Length == 4) // for 4 digit
            {
                double location = Convert.ToDouble(aerofoil_tb.Text.Substring(1, 1))/10; // 2nd digit/10 gives camber location
                double maxThickness = Convert.ToDouble(aerofoil_tb.Text.Substring(2, 2))/100; // last 2 digits gives max thickness

                // display extracted information to text box
                camber_tb.Text = aerofoil_tb.Text.Substring(0, 1) + "%";
                designCL_tb.Text = "N/A";
                location_tb.Text = location.ToString();
                maxThickness_tb.Text = maxThickness.ToString();
                m_tb.Text = "N/A";
                k_tb.Text = "N/A";
            }
            else
                aerofoil_tb.Text = "invalid"; // if entered aerofoil are not 4 or 5 digits, display invalid
        }

        private void analytical_bt_Click(object sender, EventArgs e) // for updating analytical calculation
        {
            aerofoilData_bt_Click(sender, e); // update the aerofoil data section accordingly

            if (aerofoil_tb.Text.Length != 5 && aerofoil_tb.Text.Length != 4) // if entered aerofoil are not 4 or 5 digits, display invalid
            {
                aerofoil_tb.Text = "invalid";
            }
            else
            {
                double location = Convert.ToDouble(aerofoil_tb.Text.Substring(1, 1)) / 20; // location for 5 digit
                double alpha = Convert.ToDouble(alpha_tb.Text) * Math.PI / 180; // change input AoA deg to rads

                double m = 0, k = 0, limit = 0;

                // m & k values have different definitions for 4 & 5 digit
                if (aerofoil_tb.Text.Length == 5)
                {
                    m = MValue(location, aerofoil_tb.Text[2]);
                    k = KValue(location, aerofoil_tb.Text[2]);
                    limit = Math.Acos(1 - 2 * m);
                }
                else if (aerofoil_tb.Text.Length == 4)
                {
                    m = Convert.ToDouble(aerofoil_tb.Text.Substring(0, 1)) / 100; // %camber
                    k = Convert.ToDouble(aerofoil_tb.Text.Substring(1, 1)) / 10; // location of camber for 4 digit
                    limit = Math.Acos(1 - 2 * k);
                }
                else // if entered aerofoil are not 4 or 5 digits, display invalid
                {
                    aerofoil_tb.Text = "invalid";
                }

                // find integration lim cartesian to polar based on m value
                //intLim_tb.Text = limit.ToString();

                // general structure for A coefficient calculation is same for 4 & 5 digit
                double k2k1; // define k2/k1 for coefficient calculations
                if (aerofoil_tb.Text.Length == 4 || aerofoil_tb.Text[2] == '0') { k2k1 = 0; }// define k2/k1, if standard 5 digit, pass 0
                else k2k1 = K2K1Value(location, aerofoil_tb.Text[2]); // otherwise provide k2/k1

                // calculate A0, A1, A2
                double A0_integral = (1 / Math.PI) * (A0_fore(m, k, limit, k2k1) - A0_fore(m, k, 0, k2k1) + A0_aft(m, k, Math.PI, k2k1) - A0_aft(m, k, limit, k2k1));
                double A0 = alpha - A0_integral;
                double A1 = (2 / Math.PI) * (A1_fore(m, k, limit, k2k1) - A1_fore(m, k, 0, k2k1) + A1_aft(m, k, Math.PI, k2k1) - A1_aft(m, k, limit, k2k1));
                double A2 = (2 / Math.PI) * (A2_fore(m, k, limit, k2k1) - A2_fore(m, k, 0, k2k1) + A2_aft(m, k, Math.PI, k2k1) - A2_aft(m, k, limit, k2k1));

                //A0_int.Text = A0_integral.ToString();
                a0_tb.Text = Convert.ToString(A0);
                a1_tb.Text = Convert.ToString(A1);
                a2_tb.Text = Convert.ToString(A2);

                //a2int_fore.Text = Convert.ToString(A2_fore(m, k, limit, k2k1) - A2_fore(m, k, 0, k2k1));
                //a2int_aft.Text = Convert.ToString(A2_aft(m, k, Math.PI, k2k1) - A2_aft(m, k, limit, k2k1));

                double CL = Math.PI * (2 * A0 + A1); // coefficient of lift
                double CM = -Math.PI * (A0 + A1 - A2 / 2) / 2; // CM about leading edge
                double CMac = -Math.PI * (A1 - A2) / 4; // CM about quarter chord
                double alpha_zl = (A0_integral - A1 / 2) * 180 / Math.PI; //turn into degrees, derived from Cl equation for when Cl = 0

                cl_tb.Text = CL.ToString();
                cm_tb.Text = CM.ToString();
                cmac_tb.Text = CMac.ToString();
                alphazl_tb.Text = alpha_zl.ToString();
            }
        }

        #region A-Coefficients 4 & 5 digits

        double AA(double m, double k) // integral part A, standard, fore
        {
            return k / 8 - k * m / 2 + k * Math.Pow(m, 2) * (3 - m) / 6;
        }

        double AA(double m, double k, double k2k1) // integral part A, reflex, including k2/k1, fore
        {
            return k / 8 - k * m / 2 + k * (3 * Math.Pow(m, 2) - k2k1 * Math.Pow(1 - m, 3) - Math.Pow(m, 3)) / 6;
        }

        double AAaft(double m, double k, double k2k1) // integral part A, reflex, including k2/k1, aft
        {
            return k2k1 * (k / 8 - k * m / 2 + k * Math.Pow(m, 2) / 2) 
                - k * (k2k1 * Math.Pow(1 - m, 3) + Math.Pow(m, 3)) / 6;
        }

        double BB(double m, double k) // integral part B, standard/reflex
        {
            return k * m / 2 - k / 4;
        }

        double CC(double m, double k) // integral part C, standard/reflex
        {
            return k / 8;
        }

        // A0
        double A0dx_f(double m, double k, double limit, double k2k1) // fore, numerical
        {
            if (aerofoil_tb.Text.Length == 5)
            {
                double A;
                if (k2k1 == 0){ A = AA(m, k); } // if non-reflex, pass k2k1 == 0
                else { A = AA(m, k, k2k1); } // if k2k1 =/= 0, calculate using reflex method

                double B = BB(m, k); double C = CC(m, k);

                return A + B * Math.Cos(limit) + C * Math.Pow(Math.Cos(limit), 2);
            }
            else
                return m * (2 * k - 1) / Math.Pow(k, 2) + m * Math.Cos(limit) / Math.Pow(k, 2);
        }

        double A0_fore(double m, double k, double limit, double k2k1) // fore, analytical
        {
            if (aerofoil_tb.Text.Length == 5)
            {
                double A;
                if (k2k1 == 0) { A = AA(m, k); } // if non-reflex, pass k2k1 == 0
                else { A = AA(m, k, k2k1); } // if k2k1 =/= 0, calculate using reflex method
                
                double B = BB(m, k); double C = CC(m, k);

                return A * limit + B * Math.Sin(limit) + C * (limit / 2 + Math.Sin(2 * limit) / 4);
            }
            else // 4 digit
                return m * limit * (2 * k - 1) / Math.Pow(k, 2) + (m / Math.Pow(k, 2)) * Math.Sin(limit);
        }

        double A0_fore(double m, double k, double a, double b, double k2k1) // fore, numerical, a & b are integration limits
        {
            return ((b - a) / 6) * (A0dx_f(m, k, a, k2k1) + 4 * A0dx_f(m, k, ((a + b) / 2), k2k1) + A0dx_f(m, k, b, k2k1));
        }

        double A0dx_a(double m, double k, double limit, double k2k1) // aft, numerical
        {
            if (aerofoil_tb.Text.Length == 5)
            {
                if (k2k1 == 0) // standard
                {
                    double D = -k * Math.Pow(m, 3) / 6;
                    return D;
                }
                else // reflex
                {
                    double A = AAaft(m, k, k2k1); double B = k2k1 * BB(m, k); double C = k2k1 * CC(m, k);
                    return A + B * Math.Cos(limit) + C * Math.Pow(Math.Cos(limit), 2);
                }                
            }
            else
                return m * (2 * k - 1) / Math.Pow(1 - k, 2) + m * Math.Cos(limit) / Math.Pow(1 - k, 2);
        }

        double A0_aft(double m, double k, double limit, double k2k1) // aft, analytical 
        {
            if (aerofoil_tb.Text.Length == 5)
            {
                if (k2k1 == 0) // standard
                {
                    double D = -k * Math.Pow(m, 3) / 6;
                    return D * limit;
                }
                else // reflex
                {
                    double A = AAaft(m, k, k2k1); double B = k2k1 * BB(m, k); double C = k2k1 * CC(m, k);
                    return A * limit + B * Math.Sin(limit) + C * (limit / 2 + Math.Sin(2 * limit) / 4);
                }
            }
            else
                return m * limit * (2 * k - 1) / Math.Pow(1 - k, 2) + (m / Math.Pow(1 - k, 2)) * Math.Sin(limit);
        }

        double A0_aft(double m, double k, double a, double b, double k2k1) // aft, numerical
        {
            return ((b - a) / 6) * (A0dx_a(m, k, a, k2k1) + 4 * A0dx_a(m, k, ((a + b) / 2), k2k1) + A0dx_a(m, k, b, k2k1));
        }// end A0

        // A1
        double A1dx_f(double m, double k, double limit, double k2k1) // fore, numerical
        {
            if (aerofoil_tb.Text.Length == 5)
            {
                double A;
                if (k2k1 == 0) { A = AA(m, k); } // if non-reflex, pass k2k1 == 0
                else { A = AA(m, k, k2k1); } // if k2k1 =/= 0, calculate using reflex method
                
                double B = BB(m, k); double C = CC(m, k);

                return A * Math.Cos(limit) + B * Math.Pow(Math.Cos(limit), 2) + C * Math.Pow(Math.Cos(limit), 3);
            }
            else
                return m * (2 * k - 1) * Math.Cos(limit) / Math.Pow(k, 2) 
                    + m * Math.Pow(Math.Cos(limit), 2) / Math.Pow(k, 2);
        }

        double A1_fore(double m, double k, double limit, double k2k1) // fore, analytical
        {
            if (aerofoil_tb.Text.Length == 5)
            {
                double A;
                if (k2k1 == 0) { A = AA(m, k); } // if non-reflex, pass k2k1 == 0
                else { A = AA(m, k, k2k1); } // if k2k1 =/= 0, calculate using reflex method

                double B = BB(m, k); double C = CC(m, k);

                return A * Math.Sin(limit) + B * (limit / 2 + Math.Sin(2 * limit) / 4) + C * (Math.Sin(limit) - Math.Pow(Math.Sin(limit), 3) / 3);
            }
            else
                return m * (2 * k - 1) * Math.Sin(limit) / Math.Pow(k, 2) 
                    + m * (limit / 2 + 0.25 * Math.Sin(2 * limit)) / Math.Pow(k, 2);
        }

        double A1_fore(double m, double k, double a, double b, double k2k1) // fore, numerical
        {
            return ((b - a) / 6) * (A1dx_f(m, k, a, k2k1) + 4 * A1dx_f(m, k, ((a + b) / 2), k2k1) + A1dx_f(m, k, b, k2k1));
        }

        double A1dx_a(double m, double k, double limit, double k2k1) // aft, numerical
        {
            if (aerofoil_tb.Text.Length == 5)
            {
                if (k2k1 == 0) // standard
                {
                    double D = -k * Math.Pow(m, 3) / 6;
                    return D * Math.Cos(limit);
                }
                else // reflex
                {
                    double A = AAaft(m, k, k2k1); double B = k2k1 * BB(m, k); double C = k2k1 * CC(m, k);
                    return A * Math.Cos(limit) + B * Math.Pow(Math.Cos(limit), 2) + C * Math.Pow(Math.Cos(limit), 3);
                }
            }
            else
                return m * (2 * k - 1) * Math.Cos(limit) / Math.Pow(1 - k, 2)
                    + m * Math.Pow(Math.Cos(limit), 2) / Math.Pow(1 - k, 2);
        }
        double A1_aft(double m, double k, double limit, double k2k1) // aft, analytical 
        {
            if (aerofoil_tb.Text.Length == 5)
            {
                if (k2k1 == 0) // standard
                {
                    double D = -k * Math.Pow(m, 3) / 6;
                    return D * Math.Sin(limit);
                }
                else // reflex
                {
                    double A = AAaft(m, k, k2k1); double B = k2k1 * BB(m, k); double C = k2k1 * CC(m, k);
                    return A * Math.Sin(limit) + B * (limit / 2 + Math.Sin(2 * limit) / 4) + C * (Math.Sin(limit) - Math.Pow(Math.Sin(limit), 3) / 3);
                }            
            }
            else
                return m * (2 * k - 1) * Math.Sin(limit) / Math.Pow(1 - k, 2) 
                    + m * (limit / 2 + 0.25 * Math.Sin(2 * limit)) / Math.Pow(1 - k, 2);
        }

        double A1_aft(double m, double k, double a, double b, double k2k1) // aft, numerical
        {
            return ((b - a) / 6) * (A1dx_a(m, k, a, k2k1) + 4 * A1dx_a(m, k, ((a + b) / 2), k2k1) + A1dx_a(m, k, b, k2k1));
        }// end A1

        // A2
        double A2dx_f(double m, double k, double limit, double k2k1) // fore, numerical
        {
            if (aerofoil_tb.Text.Length == 5)
            {
                double A;
                if (k2k1 == 0) { A = AA(m, k); } // if non-reflex, k2k1 == 0 is passed
                else { A = AA(m, k, k2k1); } // if k2k1 =/= 0, calculate using reflex method

                double B = BB(m, k); double C = CC(m, k);

                return A * Math.Cos(2 * limit)
                    + B * Math.Cos(limit) * Math.Cos(2 * limit)
                    + C * Math.Pow(Math.Cos(limit), 2) * Math.Cos(2 * limit);
            }
            else
                return m * (2 * k - 1) * Math.Cos(2 * limit) / Math.Pow(k, 2)
                    + m * Math.Cos(limit) * Math.Cos(2 * limit) / Math.Pow(k, 2);
        }

        double A2_fore(double m, double k, double limit, double k2k1) // fore, analytical
        {
            if (aerofoil_tb.Text.Length == 5)
            {
                double A;
                if (k2k1 == 0) { A = AA(m, k); } // if non-reflex, k2k1 == 0 is passed, A calc doesnt involve k2/k1
                else { A = AA(m, k, k2k1); } // if k2k1 =/= 0, calculate using reflex method

                double B = BB(m, k); double C = CC(m, k);

                return A * Math.Sin(2 * limit) / 2 
                    + B * (Math.Sin(3 * limit) / 6 + Math.Sin(limit) / 2) 
                    + C * (limit / 4 + Math.Sin(2 * limit) / 4 + Math.Sin(4 * limit) / 16);
            }
            else
                return m * (2 * k - 1) * 0.5 * Math.Sin(2 * limit) / Math.Pow(k, 2) 
                    + m * (Math.Sin(3 * limit) / 6 + 0.5 * Math.Sin(limit)) / Math.Pow(k, 2);
        }

        double A2_fore(double m, double k, double a, double b, double k2k1) // fore, numerical
        {
            return ((b - a) / 6) * (A2dx_f(m, k, a, k2k1) + 4 * A2dx_f(m, k, ((a + b) / 2), k2k1) + A2dx_f(m, k, b, k2k1));
        }

        double A2dx_a(double m, double k, double limit, double k2k1) // aft, numerical
        {
            if (aerofoil_tb.Text.Length == 5)
            {
                if (k2k1 == 0) // standard
                {
                    double D = -k * Math.Pow(m, 3) / 6;
                    return D * Math.Cos(2 * limit);
                }
                else // reflex, same as fore, except the A B C coefficients
                {
                    double A = AAaft(m, k, k2k1); double B = k2k1 * BB(m, k); double C = k2k1 * CC(m, k);
                    return A * Math.Cos(2 * limit)
                    + B * Math.Cos(limit) * Math.Cos(2 * limit)
                    + C * Math.Pow(Math.Cos(limit), 2) * Math.Cos(2 * limit);
                }
            }
            else // 4 digit
                return m * (2 * k - 1) * Math.Cos(2 * limit) / Math.Pow(1 - k, 2)
                    + m * Math.Cos(limit) * Math.Cos(2 * limit) / Math.Pow(1 - k, 2);
        }

        double A2_aft(double m, double k, double limit, double k2k1) // aft, analytical 
        {
            if (aerofoil_tb.Text.Length == 5)
            {
                if (k2k1 == 0) // standard
                {
                    double D = -k * Math.Pow(m, 3) / 6;
                    return D * Math.Sin(2 * limit) / 2;
                }
                else // reflex
                {
                    double A = AAaft(m, k, k2k1); double B = k2k1 * BB(m, k); double C = k2k1 * CC(m, k);
                    return A * Math.Sin(2 * limit) / 2
                    + B * (Math.Sin(3 * limit) / 6 + Math.Sin(limit) / 2)
                    + C * (limit / 4 + Math.Sin(2 * limit) / 4 + Math.Sin(4 * limit) / 16);
                }
            }
            else
                return m * (2 * k - 1) * 0.5 * Math.Sin(2 * limit) / Math.Pow(1 - k, 2) 
                    + m * (Math.Sin(3 * limit) / 6 + 0.5 * Math.Sin(limit)) / Math.Pow(1 - k, 2);
        }

        double A2_aft(double m, double k, double a, double b, double k2k1) // aft, numerical
        {
            return ((b - a) / 6) * (A2dx_a(m, k, a, k2k1) + 4 * A2dx_a(m, k, ((a + b) / 2), k2k1) + A2dx_a(m, k, b, k2k1));
        }// end A2
        #endregion

        private void numerical_bt_Click(object sender, EventArgs e) // for updating numerical calculation
        {
            aerofoilData_bt_Click(sender, e); // update the data section accordingly
            
            if (aerofoil_tb.Text.Length != 5 && aerofoil_tb.Text.Length != 4)
            {
                aerofoil_tb.Text = "invalid";
            }
            else
            {
                double location = Convert.ToDouble(aerofoil_tb.Text.Substring(1, 1)) / 20;
                double alpha = Convert.ToDouble(alpha_tb.Text) * Math.PI / 180; // change input AoA deg to rads

                double m = 0, k = 0, limit = 0;

                // m & k values have different definitions for 4 & 5 digit
                if (aerofoil_tb.Text.Length == 5)
                {
                    m = MValue(location, aerofoil_tb.Text[2]);
                    k = KValue(location, aerofoil_tb.Text[2]);
                    limit = Math.Acos(1 - 2 * m);
                }
                else
                {
                    m = Convert.ToDouble(aerofoil_tb.Text.Substring(0, 1)) / 100; // %camber
                    k = Convert.ToDouble(aerofoil_tb.Text.Substring(1, 1)) / 10; // location of camber for 4 digit
                    limit = Math.Acos(1 - 2 * k);
                }

                // general structure for A coefficient calculation is same for 4 & 5 digit
                double k2k1;
                if (aerofoil_tb.Text.Length == 4 || aerofoil_tb.Text[2] == '0') { k2k1 = 0; }// define k2/k1, if standard 5 digit or 4 digit, pass 0
                else k2k1 = K2K1Value(location, aerofoil_tb.Text[2]); // otherwise provide k2/k1 for reflex calculation

                // calculate A0, A1, A2
                double A0_integral = (1 / Math.PI) * (A0_fore(m, k, 0, limit, k2k1) + A0_aft(m, k, limit, Math.PI, k2k1));
                double A0 = alpha - A0_integral;
                double A1 = (2 / Math.PI) * (A1_fore(m, k, 0, limit, k2k1) + A1_aft(m, k, limit, Math.PI, k2k1));
                double A2 = (2 / Math.PI) * (A2_fore(m, k, 0, limit, k2k1) + A2_aft(m, k, limit, Math.PI, k2k1));

                //A0_int.Text = A0_integral.ToString();
                a0_tb.Text = Convert.ToString(A0);
                a1_tb.Text = Convert.ToString(A1);
                a2_tb.Text = Convert.ToString(A2);

                //a2int_fore.Text = Convert.ToString(A2_fore(m, k, 0, limit, k2k1));
                //a2int_aft.Text = Convert.ToString(A2_aft(m, k, limit, Math.PI, k2k1));

                double CL = Math.PI * (2 * A0 + A1);
                double CM = -Math.PI * (A0 + A1 - A2 / 2) / 2; // CM about leading edge
                double CMac = -Math.PI * (A1 - A2) / 4; // CM about quarter chord
                double alpha_zl = (A0_integral - A1 / 2) * 180 / Math.PI; //turn into degrees, derived from Cl equation for when Cl = 0

                cl_tb.Text = CL.ToString();
                cm_tb.Text = CM.ToString();
                cmac_tb.Text = CMac.ToString();
                alphazl_tb.Text = alpha_zl.ToString();
            }
            
        }


        // Plot Aerofoil

        private void plot_bt_Click(object sender, EventArgs e)
        {
            aerofoilData_bt_Click(sender, e); // update aerufoil info

            // Find center of plotting canvas
            center_x = panel_plot.Width / 2;
            center_y = panel_plot.Height / 2;

            panel_plot.Refresh();

            if (aerofoil_tb.Text.Length != 5 && aerofoil_tb.Text.Length != 4) // not valid for digits other than 4/5
            {
                aerofoil_tb.Text = "invalid";
            }
            else // call methods to draw camberline, upper, and lower sections
            {
                drawGrid();
                drawCamberline();
                drawUpper();
                drawLower();
            }
        }

        // initialise pen properties and graphics
        Pen myPen = new Pen(Color.Black);
        Pen myPenCamber = new Pen(Color.Green);
        Pen myPenGrid = new Pen(Color.FromArgb(255, 140, 140, 140));
        Graphics g = null;

        static int center_x, center_y;
        static double start_xc, start_yc;
        static double end_xc, end_yc;

        static double increment = 0.005; // set resolution for plotting, smaller number for higher res

        double thicknessDistribution(double x) // same for 4 & 5 digit
        {
            double maxThickness;
            
            if (aerofoil_tb.Text.Length == 5) // extract max thickness for 5 digit
            {
                maxThickness = Convert.ToDouble(aerofoil_tb.Text.Substring(3, 2)) / 100; //last 2 digits;
            }
            else // 4 digit
            {
                maxThickness = Convert.ToDouble(aerofoil_tb.Text.Substring(2, 2)) / 100;
            }

            return maxThickness * (0.2969 * Math.Sqrt(x) - 0.126 * x - 0.3516 * Math.Pow(x, 2) + 0.2843 * Math.Pow(x, 3) - 0.1015 * Math.Pow(x, 4))/0.2;
        }

        // calculate theta for plotting aerofoil upper lower from gradient of camber dy/dx
        double theta(double x) 
        {
            double gradient;

            if (aerofoil_tb.Text.Length == 5) // 5 digit
            {
                double location = Convert.ToDouble(aerofoil_tb.Text.Substring(1, 1)) / 20;
                double k = KValue(location, aerofoil_tb.Text[2]);
                double m = MValue(location, aerofoil_tb.Text[2]);

                if (x < m)
                {
                    gradient = k * (3 * Math.Pow(x, 2) - 6 * m * x + Math.Pow(m, 2) * (3 - m)) / 6;
                }
                else
                    gradient = -k * Math.Pow(m, 3) / 6;
            }
            else // 4 digit
            {
                double m = Convert.ToDouble(aerofoil_tb.Text.Substring(0, 1)) / 100; // %camber
                double k = Convert.ToDouble(aerofoil_tb.Text.Substring(1, 1)) / 10; // location of camber for 4 digit

                if (x < k) // k used as p
                {
                    gradient = 2 * m / k - 2 * m * x / Math.Pow(k, 2);
                }
                else
                    gradient = (2 * k * m - 2 * m * x) / Math.Pow(1 - k, 2);
            }
            return Math.Atan(gradient);
        } //
        
        // methods for upper lower aerofoil x y coordinates, same for 4, 5 digit
        double xl(double x)
        {
            return x - thicknessDistribution(x) * Math.Sin(theta(x));
        }

        double xu(double x)
        {
            return x + thicknessDistribution(x) * Math.Sin(theta(x));
        }

        double yl(double x)
        {
            return camber_plot(x) - thicknessDistribution(x) * Math.Cos(theta(x));
        }

        double yu(double x)
        {
            return camber_plot(x) + thicknessDistribution(x) * Math.Cos(theta(x));
        } //

        // calculate y coordinate for camber line fore and aft
        double camber_plot(double x)
        {
            if (aerofoil_tb.Text.Length == 5) // 5 digit camber
            {
                double location = Convert.ToDouble(aerofoil_tb.Text.Substring(1, 1)) / 20;

                if (aerofoil_tb.Text[2] == '0') // standard camber equations
                {
                    
                    double k = KValue(location, aerofoil_tb.Text[2]);
                    double m = MValue(location, aerofoil_tb.Text[2]);

                    if (x < m) // fore
                    {
                        return k * (Math.Pow(x, 3) - 3 * m * Math.Pow(x, 2) + Math.Pow(m, 2) * (3 - m) * x) / 6;
                    }
                    else // aft
                        return k * Math.Pow(m, 3) * (1 - x) / 6;
                }

                else // reflex camber equations
                {
                    double k = KValue(location, aerofoil_tb.Text[2]); // KValue implements reflex value (k1 value)
                    double m = MValue(location, aerofoil_tb.Text[2]); // MValue implements reflex value
                    double k2k1 = K2K1Value(location, aerofoil_tb.Text[2]); //K2/K1 value

                    if (x < m) // fore
                    {
                        return k * (Math.Pow(x - m, 3) - k2k1 * Math.Pow(1 - m, 3) * x - Math.Pow(m, 3) * x + Math.Pow(m, 3)) / 6;
                    }
                    else // aft
                        return k * (k2k1 * Math.Pow(x - m, 3) - k2k1 * Math.Pow(1 - m, 3) * x - Math.Pow(m, 3) * x + Math.Pow(m, 3)) / 6;
                }
            }
            else // 4 digit camber equations
            {
                double m = Convert.ToDouble(aerofoil_tb.Text.Substring(0, 1)) / 100; // %camber
                double k = Convert.ToDouble(aerofoil_tb.Text.Substring(1, 1)) / 10; // location of camber for 4 digit

                if (x < k) // fore
                {
                    return 2 * m * x / k - m * Math.Pow(x, 2) / Math.Pow(k, 2);
                }
                else // aft
                    return m * ((1 - 2 * k) + 2 * k * x - Math.Pow(x, 2)) / Math.Pow(1 - k, 2);
            }
        }//
        
        private void drawCamberline() // draw camber line
        {
            int increment_num;
            increment_num = Convert.ToInt32(1 / increment) + 1; // number of points to make up the plot line determined by increment size
            double[] points_x = new double[increment_num]; // declare array holder for total points requried along x axis

            for (int i = 0; i < increment_num; i++) 
            {
                points_x[i] = i * increment; // define x coord in point/pixel between 0 - 1, store all points in array
            }

            foreach (var x in points_x) // repeatedly connect 2 points based on camber equation and increment size
            {
                double y = camber_plot(x); // calculate y coord of 1st point of segment/increment based on defined x
                double y2 = camber_plot(x + increment); // y coord of 2nd point of segment/increment
                start_xc = 0.1 * panel_plot.Width + x * (0.8*panel_plot.Width); // leave 10% space from left, scale non-dim x coord to 80% panel width for 1st point of segment
                start_yc = center_y - y * (0.8 * panel_plot.Width); // starting at mid-canvas vertically, find y position of 1st point of segment
                end_xc = 0.1 * panel_plot.Width + (x + increment) * (0.8*panel_plot.Width); // scale non-dim x coord for 2nd point
                end_yc = center_y - y2 * (0.8 * panel_plot.Width); // find y position of 2nd point of segment

                PointF[] points_c = // define points with float number coordinates
                {
                new PointF((float)start_xc, (float)start_yc),
                new PointF((float)end_xc, (float)end_yc)
                };

                g.DrawLines(myPenCamber, points_c); // draw all segments of lines
            }
        }//

        private void drawUpper() // draw upper curve
        {
            int increment_num;
            increment_num = Convert.ToInt32(1 / increment) + 1; // number of points to make up the plot line determined by increment size
            double[] points_x = new double[increment_num]; // declare array holder for total points requried along x axis

            for (int i = 0; i < increment_num; i++)
            {
                points_x[i] = i * increment; // define x coord in point/pixel between 0 - 1, store all points in array
            }

            foreach (var x in points_x) // repeatedly connect 2 points based on camber equation and increment size
            {
                // based on defined point array, find corresponding point positions that connects aerofoil plot
                // for each point, 2 points are derived to form segment
                double x1 = xu(x);
                double x2 = xu(x + increment);
                double y1 = yu(x);
                double y2 = yu(x + increment);
                start_xc = 0.1 * panel_plot.Width + x1 * (0.8 * panel_plot.Width); // leave 10% space from left, scale non-dim x coord to 80% panel width for 1st point of segment
                start_yc = center_y - y1 * (0.8 * panel_plot.Width); // starting at mid-canvas vertically, find y position of 1st point of segment
                end_xc = 0.1 * panel_plot.Width + x2 * (0.8 * panel_plot.Width); // scale non-dim x coord for 2nd point
                end_yc = center_y - y2 * (0.8 * panel_plot.Width); // find y position of 2nd point of segment

                PointF[] points_u = // define points with float number coordinates
                {
                new PointF((float)start_xc, (float)start_yc),
                new PointF((float)end_xc, (float)end_yc)
                };

                g.DrawLines(myPen, points_u); // draw all segments of lines
            }
        }

        private void drawLower() // draw lower curve, methods the same as upper curve, but implemented euqations for calculating lower point coordinates
        {
            int increment_num;
            increment_num = Convert.ToInt32(1 / increment) + 1;
            double[] points_x = new double[increment_num];

            for (int i = 0; i < increment_num; i++)
            {
                points_x[i] = i * increment;
            }

            foreach (var x in points_x)
            {
                double x1 = xl(x);
                double x2 = xl(x + increment);
                double y1 = yl(x);
                double y2 = yl(x + increment);
                start_xc = 0.1 * panel_plot.Width + x1 * (0.8 * panel_plot.Width);
                start_yc = center_y - y1 * (0.8 * panel_plot.Width);
                end_xc = 0.1 * panel_plot.Width + x2 * (0.8 * panel_plot.Width);
                end_yc = center_y - y2 * (0.8 * panel_plot.Width);

                PointF[] points_l =
                {
                new PointF((float)start_xc, (float)start_yc),
                new PointF((float)end_xc, (float)end_yc)
                };

                g.DrawLines(myPen, points_l);
            }
        }

        private void drawGrid()
        {
            double increment_len;
            int res = 10;
            increment_len = 0.8 * panel_plot.Width / res; // number of points to make up the plot line determined by increment size
            double[] points_x = new double[res+1]; // declare array holder for total points requried along x axis

            for (int i = 0; i < res+1; i++)
            {
                points_x[i] = i * increment_len; // define x coord in point/pixel between 0 - 1, store all points in array
            }

            foreach (var x in points_x) // repeatedly connect 2 points based on camber equation and increment size
            {
                double y1 = center_y - 2 * increment_len;
                double y2 = center_y + 2 * increment_len;

                PointF[] points_u = // define points with float number coordinates
                {
                new PointF((float)(x + 0.1 * panel_plot.Width), (float)y1),
                new PointF((float)(x + 0.1 * panel_plot.Width), (float)y2)
                };

                g.DrawLines(myPenGrid, points_u); // draw all segments of lines
            }

            double[] points_y = new double[5]; // declare array holder for total points requried along x axis

            for (int i = 0; i < 5; i++)
            {
                points_y[i] = (i - 2) * increment_len + center_y; // define x coord in point/pixel between 0 - 1, store all points in array
            }

            foreach (var y in points_y) // repeatedly connect 2 points based on camber equation and increment size
            {
                double x1 = 0.1 * panel_plot.Width;
                double x2 = 0.9 * panel_plot.Width;

                PointF[] points_h = // define points with float number coordinates
                {
                new PointF((float)(x1), (float)(y)),
                new PointF((float)(x2), (float)(y))
                };

                g.DrawLines(myPenGrid, points_h); // draw all segments of lines
            }
        }

        private void panel_plot_Paint(object sender, PaintEventArgs e) // plot on canvas
        {
            myPen.Width = 1; // pen thickness
            g = panel_plot.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // smoothout the jagged connection of points
        }

        private void exit_bt_Click(object sender, EventArgs e) // exit programme
        {
            Application.Exit();
        }
    }
}
