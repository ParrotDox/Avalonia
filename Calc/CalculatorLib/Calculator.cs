using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Markup;

namespace CalculatorLib
{
    public class Calculator
    {
        //Operations:∨, ∧, ¬, ⊕, →, ~, ↓, ↑
        //Operations: +, *, ¬, O, →, ~, ↓, ↑
        //Example: (x → y) ~ (y → z)
        public string equation = "";
        char[] operations = new char[] { '+', '*', '¬', 'O', '→', '~', '↓', '↑' };
        char[] varLetters = new char[] { 'x', 'y', 'z', 'k', 'r' };
        public List<char> variables;
        public string[,] truthTable;
        public string PDNF;
        public string PCNF;
        public string MDNF;

        //[EQUATION SETTER FOR AVALONIA]
        public string InitEquation(string eq) 
        {
            string tempEq = eq;
            tempEq = tempEq.Replace(" ", "");
            equation = tempEq;
            return tempEq;
        }
        //[CORRECTNESS CHECKERS]
        public bool CheckBrackets(string eq)
        {
            //!!!USE InitEquation before using this method!!!
            //Method is used to check the correctness of the equation brackets
            int balance = 0;
            foreach (char letter in eq)
            {
                if (letter == '(')
                    ++balance;
                if (letter == ')')
                    --balance;
                //If the order of brackets is incorrect
                if (balance < 0)
                    return false;
            }
            if (balance == 0)
                return true;
            return false;
        }
        public bool CheckVariables(string eq) 
        {
            //Checks if equation has mistakes such as "xx" or "rr"
            bool flag = true;
            foreach (char variable in variables) 
            {
                string stringVar = variable.ToString();
                string incorrectCombination = stringVar + stringVar;
                if (eq.Contains(incorrectCombination))
                    flag = false;
            }
            return flag;
        }
        public bool CheckOperations(string eq)
        {
            //Checks if equation has mistakes such as "++" or "+*"
            bool flag = true;
            foreach(char operation1 in operations) 
            {
                if(eq.Contains(operation1 + " ")) 
                {
                    flag = false;
                    break;
                }
                foreach (char operation2 in operations)
                {
                    string stringOperation1 = operation1.ToString();
                    string stringOperation2 = operation2.ToString();
                    string incorrectCombination = stringOperation1 + stringOperation2;
                    if(eq.Contains(incorrectCombination))
                        flag = false; 
                }
            }
            return flag;
        }
        //[SETTERS FOR CLASS FIELDS]
        public List<char> InitVariables(string eq) 
        {
            //Method is used to find what variables are in equation
            List<char> temp = new List<char>();
            foreach (char letter in eq) 
            {
                if(varLetters.Contains(letter) && !temp.Contains(letter))
                    temp.Add(letter);
            }
            temp.Sort();
            variables = new List<char>(temp);
            return temp;
        }
        //[EQUATION ANALYZING METHODS]
        public bool GetPartialResult(string eqPart) 
        {
            //Method solves the part of equation returning it's result
            //  Example of what we sent: 0→1
            //  Here we get boolean values from string partial equation
            //      Left value
            bool left = false;
            char operation = '?';
            bool right = false;
            //      If equation consists of two operands
            if (eqPart.Length == 3) 
            {
                operation = eqPart[1];
                if (eqPart[0] == '1')
                    left = true;
                else
                    left = false;
                if (eqPart.Length == 3)
                    if (eqPart[2] == '1')
                        right = true;
                    else
                        right = false;
            }
            //      If equation consists of one operand
            else
            {
                operation = eqPart[0];
                if (eqPart[1] == '1')
                    left = true;
                else
                    left = false;

            }
            
            bool result = false;
            switch (operation) 
            {
                case '¬': result = !left; break;
                case '+': result = left || right; break;
                case '*': result = left && right; break;
                case 'O': result = (left&&!right) || (!left&&right); break;
                case '→': result = !left || right; break;
                case '~': result = (left&&right) || (!left&&!right); break;
                case '↑': result = !left || !right; break;
                case '↓': result = !left && !right; break;
            }
            //  Example of what we get: 0→1 is TRUE
            return result;
        }
        public bool Simplify(string valuesOfVariables) 
        {
            //!!!USE InitVariables before using this method!!!
            //Method gets rid of brackets, simplifying the equation and returning result of the equation
            //  Example of what we send: (1→0)~(0→1)
            //      Replacing vars with values
            string tempEq = equation;
            //  Clearing equation from spacebars
            tempEq = tempEq.Replace(" ", "");
            for (int i = 0; i < variables.Count; ++i)
            {
                //  Replace var with value (X is 0 | Y is 1 etc.)
                tempEq = tempEq.Replace(variables[i], valuesOfVariables[i]);
            }
            //      Getting rid of brackets
            while (tempEq.Contains('(')) 
            {
                //  Indexes of the deepest brackets "(" and ")"
                int start = tempEq.LastIndexOf('(');    //Last left bracket
                int end = tempEq.IndexOf(')', start);   //Last right bracket

                //  Inner Equation
                string partialEq = tempEq.Substring(start + 1, end - start - 1);

                //  Getting Partial result
                bool partialRes = GetPartialResult(partialEq);

                //  Replacing substring with partialResult
                tempEq = tempEq.Substring(0, start) + (partialRes ? "1":"0") + tempEq.Substring(end + 1);
            }
            //  Example of what we get: (1→0)~(0→1) is FALSE 
            //      If no brackets have left returning result of the equation 
            return GetPartialResult(tempEq);
        }
        public string[,] InitTruthTable() 
        {
            //!!!USE InitVariables before using this method!!!
            //Method is used to create a truth table and return it
            //Method bases on variables var and equation var in this class
            //  One +row is for header
            int rows = 1 + (1 * (int)Math.Pow(2, variables.Count));
            //  One +column is for result
            int columns = variables.Count + 1;


            string[,] tempTable = new string[rows, columns];
            byte bCtr = 0;
            
            //Iterating through table
            for(int row = 0; row < rows; ++row) 
            {
                for (int col = 0; col < columns; ++col)
                {
                    //Filling the header of the table
                    //  filling variables (X | Y | Z)
                    if (row == 0 && col != columns - 1)
                    {
                        tempTable[row, col] = variables[col].ToString();
                    }
                    //  filling equation cell (x→y)
                    if (row == 0 && col == columns - 1)
                    {
                        tempTable[row, col] = equation;
                    }
                    //  filling variables variation (0 | 0 | 0)
                    //  filling variables variation (0 | 0 | 1)
                    //  filling variables variation (0 | 1 | 0)
                    //  ...
                    if (row != 0 && col != columns - 1)
                    {
                        // 10 -> "10"
                        string bStr = Convert.ToString(bCtr, 2);
                        // "10" -> "010"
                        bStr = bStr.PadLeft(variables.Count, '0');
                        tempTable[row, col] = bStr[col].ToString();
                    }
                    //  filling results
                    if (row != 0 && col == columns - 1)
                    {
                        // 10 -> "10"
                        string bStr = Convert.ToString(bCtr, 2);
                        // "10" -> "010"
                        bStr = bStr.PadLeft(variables.Count, '0');
                        tempTable[row, col] = Convert.ToString(Simplify(bStr));
                    }
                    Console.Write($"{tempTable[row, col]} |");
                }
                //Increment the byte value (000 -> 001 -> 010 -> 011 etc.)
                if (row >= 1)
                    bCtr += 1;
                Console.Write($"\n");
            }
            truthTable = tempTable;
            return tempTable;
        }
        public string GetPDNF() 
        {
            //Method is used to form a PDNF
            //Method bases on truthTable var and variables var in this class

            //  Iterating through result column to find "true results"
            List<int> trueRows = new List<int>();
            int resultCol = truthTable.GetLength(1) - 1;
            for(int row = 1; row < truthTable.GetLength(0); ++row) 
            {
                if (truthTable[row, resultCol] == "True") 
                {
                    //Adding index of "true" row
                    trueRows.Add(row);
                }
            }

            //  Iterating through trueRows to form a PDNF
            string tempPDNF = "";
            foreach (int row in trueRows) 
            {
                //Iterating through columns except result column (-1)
                for (int col = 0; col < truthTable.GetLength(1) - 1; ++col)
                {
                    //Checking if the value row is the last in the trueRows (for example trueRows = [1,5,7], if we are on 7, then "checkIfLastRow = true")
                    bool IfLastRow = trueRows.IndexOf(row) == trueRows.Count - 1 ? true : false;
                    //Checking if the value column is the last in the table column (for example X|Y|Z, if we are on Z, then "bool checkIfLastCol = true")
                    bool IfLastCol = col == truthTable.GetLength(1) - 2 ? true : false;
                    //Checking is the value "1" or "0" in the table cell
                    bool isTrue = false;
                    if (truthTable[row, col] == "1")
                        isTrue = true;

                    if (!IfLastCol) 
                    {
                        if (isTrue)
                            //result: x*
                            tempPDNF += variables[col] + "*";
                        else
                            //result: ¬x*
                            tempPDNF += "¬" + variables[col] + "*";
                    }    
                    else 
                    {
                        if (!IfLastRow) 
                        {
                            if (isTrue)
                                //result: x+
                                tempPDNF += variables[col] + "+";
                            else
                                //result: ¬x+
                                tempPDNF += "¬" + variables[col] + "+";
                        }
                        else 
                        {
                            if (isTrue)
                                //result: x
                                tempPDNF += variables[col];
                            else
                                //
                                tempPDNF += "¬" + variables[col];
                        }
                    }
                }
            }
            /*
            Console.WriteLine(tempPDNF);
            Console.WriteLine("Done");
            */
            PDNF = tempPDNF;
            return tempPDNF;
        }
        public string GetPCNF() 
        {
            //Method is used to form a PCNF
            //Method bases on truthTable var and variables var in this class

            //  Iterating through result column to find "false results"
            List<int> falseRows = new List<int>();
            int resultCol = truthTable.GetLength(1) - 1;
            for (int row = 1; row < truthTable.GetLength(0); ++row)
            {
                if (truthTable[row, resultCol] == "False")
                {
                    //Adding index of "false" row
                    falseRows.Add(row);
                }
            }

            //  Iterating through falseRows to form a PCNF
            string tempPCNF = "";
            foreach (int row in falseRows)
            {
                //Iterating through columns except result column (-1)
                for (int col = 0; col < truthTable.GetLength(1) - 1; ++col)
                {
                    //Checking if the value row is the last in the trueRows (for example trueRows = [1,5,7], if we are on 7, then "checkIfLastRow = true")
                    bool IfLastRow = falseRows.IndexOf(row) == falseRows.Count - 1 ? true : false;
                    //Checking if the value column is the last in the table column (for example X|Y|Z, if we are on Z, then "bool checkIfLastCol = true")
                    bool IfLastCol = col == truthTable.GetLength(1) - 2 ? true : false;
                    //Checking is the value "1" or "0" in the table cell
                    bool isTrue = false;
                    if (truthTable[row, col] == "1")
                        isTrue = true;

                    if (!IfLastCol)
                    {
                        if (isTrue)
                            //result: x*
                            tempPCNF += variables[col] + "*";
                        else
                            //result: ¬x*
                            tempPCNF += "¬" + variables[col] + "*";
                    }
                    else
                    {
                        if (!IfLastRow)
                        {
                            if (isTrue)
                                //result: x+
                                tempPCNF += variables[col] + "+";
                            else
                                //result: ¬x+
                                tempPCNF += "¬" + variables[col] + "+";
                        }
                        else
                        {
                            if (isTrue)
                                //result: x
                                tempPCNF += variables[col];
                            else
                                //
                                tempPCNF += "¬" + variables[col];
                        }
                    }
                }
            }

            //Now when we have equation formed on false results we have to negatiate all equation
            //  Replacing operations according to De-morgan
            tempPCNF = tempPCNF.Replace("*", "#");
            tempPCNF = tempPCNF.Replace("+", "*");
            tempPCNF = tempPCNF.Replace("#", "+");

            foreach (char variable in variables) 
            {
                string varToStr = variable.ToString();
                tempPCNF = tempPCNF.Replace("¬"+ varToStr, "#");
                tempPCNF = tempPCNF.Replace(varToStr, "¬"+varToStr);
                tempPCNF = tempPCNF.Replace("#", varToStr);
            }
            /*
            Console.WriteLine(tempPCNF);
            Console.WriteLine("Done");
            */
            PCNF = tempPCNF;
            return tempPCNF;
        }
        public string GetMDNF()
        {
            //Method is used to form a MDNF
            //Method bases on truthTable var and variables var in this class

            //  Iterating through result column to find "true results"
            List<int> trueRows = new List<int>();
            int resultCol = truthTable.GetLength(1) - 1;
            for (int row = 1; row < truthTable.GetLength(0); ++row)
            {
                if (truthTable[row, resultCol] == "True")
                {
                    //Adding index of "true" row
                    trueRows.Add(row);
                }
            }

            //  Iterating through true rows to form a MDNF
            string tempMDNF = "";
            for (int trueRow = 0; trueRow < trueRows.Count - 1; ++trueRow)
            {
                //Getting the term (For exampe ¬x¬yz from table is 001)
                string tempTerm = "";
                for (int col = 0; col < truthTable.GetLength(1) - 1; ++col)
                {
                    tempTerm += truthTable[trueRows[trueRow], col];
                }
                //Now we are iterating through next trueRows to find correct combinations of gluing
                for (int trueRowNext = trueRow + 1; trueRowNext < trueRows.Count; ++trueRowNext)
                {
                    //Getting the term2 (For exampe x¬yz from table is 101)
                    string tempTerm2 = "";
                    for (int col = 0; col < truthTable.GetLength(1) - 1; ++col)
                    {
                        tempTerm2 += truthTable[trueRows[trueRowNext], col];
                    }
                    //Now we have to compare terms and find out do they equal except 1 part
                    //(001 and 101 match the condition because of the -01 at the end are equal except 0-- and 1-- at the start)
                    int countNonEqualParts = 0;
                    for (int i = 0; i < truthTable.GetLength(1) - 1; ++i)
                    {
                        if (tempTerm[i] != tempTerm2[i])
                        {
                            ++countNonEqualParts;
                        }
                    }
                    //If the difference between the term length and equal parts is only one, then this is the match
                    if (countNonEqualParts == 1) 
                    {
                        for (int i = 0; i < truthTable.GetLength(1) - 1; ++i)
                        {
                            if (tempTerm[i] == tempTerm2[i])
                            {
                                if (tempTerm[i] == '1')
                                    tempMDNF += variables[i] + "*";
                                else
                                    tempMDNF += "¬" + variables[i] + "*";
                            }
                        }
                        tempMDNF += "+";
                    }
                }
            }
            if (tempMDNF.Length != 0)
                tempMDNF = tempMDNF.Substring(0, tempMDNF.Length - 2);

            MDNF = tempMDNF;
            return tempMDNF;
        }
        //[PRINTING TABLE]
        public string PrintTable(string[,] t) 
        {
            string[,] tableSample = t;
            int maxLength = 0;
            string result = "";
            //Получаем максимальную длину ячейки
            for(int row = 0; row < tableSample.GetLength(0); row++) 
            {
                for(int col = 0;  col < tableSample.GetLength(1); col++) 
                {
                    if (tableSample[row, col].Length > maxLength)
                        maxLength = tableSample[row, col].Length;
                }
            }

            result += new string('=', maxLength * tableSample.GetLength(1)) + "\n";
            for (int row = 0; row < tableSample.GetLength(0); row++)
            {
                
                for (int col = 0; col < tableSample.GetLength(1); col++)
                {
                    result += "|" + tableSample[row, col].PadRight(maxLength, ' ');
                }
                result += "\n" + new string('=', maxLength* tableSample.GetLength(1)) + "\n";
            }
            return result;
        }
    }
}

