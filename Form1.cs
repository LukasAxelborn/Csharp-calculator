using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace labb2___calculator
{
    public partial class Form1 : Form
    {
        private List<char> userInputList = new List<char>();
        
        private bool isInvalidSyntax = false;

        private const string InvalidSyntax = "Invalid syntax";
        private const string Cantdividewithzero = "Can't divide with 0";
        private const string TooBigNumber = "Number is to large";
    
   
        public Form1()
        {
            InitializeComponent();


            for (int i = 0; i < 16; ++i)
            {
                string name = "button" + i;
                Button button = Controls[name] as Button;
               
                button.Click += numberbutton_Click;

            }
        }

   

        //https://stackoverflow.com/a/41498296
        private void numberbutton_Click(object sender, EventArgs e)
        {

            if (isInvalidSyntax)
            {
                textBox.Clear();
                isInvalidSyntax = false;
                userInputList.Clear();

            }


            Button b = sender as Button;
            if (b == null)
                return; // you can return, or throw an exception. It depends on your requirements.
            char text = b.Text[0];


            if (!((userInputList.Count == 0) && (text.Equals('+') || text.Equals('*') || text.Equals('/'))))
            {
                userInputList.Add(text); //assuming this is your common method






                switch (userInputList.Last())
                {
                    case 'C':
                        textBox.Clear();
                        userInputList.Clear();
                        break;
                    case '=':
                        textBox.Clear();
                        toCompute();
                        userInputList.Clear();

                        //gör det möjligt att använda resultatet i nästa beräkning
                        textBox.Text.ToList<char>().ForEach(c => userInputList.Add(c));
                        break;
                    default:
                        textBox.Text += userInputList.Last();
                        break;
                }
            }
        }


        
        private void toCompute()
        {
        /*
         * function to get a result form the user input
         */

            List<string> cleanList = getACleanInput();

            //order of operation
            try
            {
                //the last value in the list will alwasy be the result
                //therfor we can stop when it is one left
                while (cleanList.Count > 2)
                {

                    if (cleanList.Contains("*"))
                    {
                        makeTheOperation(cleanList, mult, "*");
                    }
                    else if (cleanList.Contains("/"))
                    {
                        makeTheOperation(cleanList, divi, "/");

                    }
                    else if (cleanList.Contains("+"))
                    {
                        makeTheOperation(cleanList, add, "+");

                    }
                    
                    else if (cleanList.Contains("-"))
                    {
                        makeTheOperation(cleanList, sub, "-");

                    }
                    

                }
                //print the final result
                textBox.Text = cleanList.First().ToString();

            }
            catch (ArgumentOutOfRangeException)
            {
                errorMessage(InvalidSyntax);

            }
            catch (FormatException)
            {
                errorMessage(InvalidSyntax);

            }
            catch (ArgumentException)
            {
                errorMessage(Cantdividewithzero);

            }
            catch (OverflowException)
            {
                errorMessage(TooBigNumber);

            }
        }

        private void errorMessage(string error)
        {
            textBox.Text = error;
            isInvalidSyntax = true;
        }

        
        private List<string> getACleanInput()
        {
         /*
         * converts the user input from [1,1,+,2,-,1,1,=]
         * to [11,+,2,-,11]
         */

            List<string> cleanList = new List<string>();

            for (int i = 0; i < userInputList.Count; i++)
            {

                string wholenumbers = "";
                
                if (userInputList[i].ToString().Equals("-"))
                {
                    wholenumbers += userInputList[i].ToString();
                    userInputList[i] = '+';
                   
                }

                for (int j = i; char.IsDigit(userInputList[j]) && (j < userInputList.Count); j++)
                {
                    wholenumbers += userInputList[j];
                    i = j;
                }

                if (wholenumbers.Length > 0)
                {
                    cleanList.Add(wholenumbers);
                }
                else
                {
                    if (!userInputList[i].ToString().Equals("-"))
                    {
                        //är antagligen en operator
                        cleanList.Add(userInputList[i].ToString());
                    }

                }
            }
                cleanList.ForEach(Console.WriteLine);
            cleanList.Remove("=");
            return cleanList;
        }

        private void makeTheOperation(List<string> cleanList, Func<string, string, int> operation, string operatorchar)
        {
            /*
             * conducts the operations on the cleanlist, finds the index for the operator from the argument, 
             * takes the value form the rigt and left of it, performs the operations, replaces the left, right and
             * the operation character with the new value.
             * [11,+,5,-,11, *, 2] -> (11*2) -> [11, +, 5, -, 22] -> (11+5) -> [16, -, 22] -> (16-22) -> [-6]
             * 
             */

            var multiindex = cleanList.IndexOf(operatorchar);
            var valuetotheleft = multiindex - 1; 
            var valuetotheright = multiindex + 1; 
            try
            {
                var op = operation(cleanList[valuetotheleft], cleanList[valuetotheright]);
                cleanList.RemoveAt(multiindex);
                cleanList.RemoveAt(multiindex);
                cleanList[multiindex - 1] = op.ToString();

            }
            catch (ArgumentOutOfRangeException)
            {
               throw new ArgumentOutOfRangeException();

            }
            catch (FormatException)
            {
                throw new FormatException();
            }
            catch (ArgumentException)
            {
                throw new ArgumentException();
            }
            catch (OverflowException)
            {
                throw new OverflowException();
            }


        }

        
        public int add(string a, string b)
        {
            return checked(int.Parse(a) + int.Parse(b));
        }
        public int sub(string a, string b)
        {
            return checked(int.Parse(a) - int.Parse(b));
        }
        public int mult(string a, string b)
        {
            return checked(int.Parse(a) * int.Parse(b));
        }
        public int divi(string a, string b)
        {
            //no divieds with zero here!
            if(int.Parse(b) == 0) throw new ArgumentException();
            return checked(int.Parse(a) / int.Parse(b));
        }

    }
}
