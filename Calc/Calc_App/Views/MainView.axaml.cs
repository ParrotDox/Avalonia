using Avalonia.Controls;
using Avalonia.Interactivity;
using CalculatorLib;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Calc_App.Views;

public partial class MainView : UserControl
{
    Calculator calculator = new Calculator();
    public MainView()
    {
        InitializeComponent();
    }
    //Variables Input
    public void ButtonX_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "X";
    }
    public void ButtonY_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "Y";
    }
    public void ButtonZ_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "Z";
    }
    public void ButtonK_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "K";
    }
    public void ButtonR_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "R";
    }
    public void ButtonLB_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "(";
    }
    public void ButtonRB_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += ")";
    }

    //Operations Input (+ * ¬ ⊕ → ↓ ↑ ~)
    public void ButtonOR_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "+";
    }
    public void ButtonAND_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "*";
    }
    public void ButtonNOT_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "¬";
    }
    public void ButtonXOR_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "O";
    }
    public void ButtonIMP_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "→";
    }
    public void ButtonNOR_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "↓";
    }
    public void ButtonNAND_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "↑";
    }
    public void ButtonXNOR_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Text += "~";
    }

    //Process Input
    public void ButtonClear_Click(object sender, RoutedEventArgs args)
    {
        EquationInput.Clear();
    }
    public void ButtonCalculate_Click(object sender, RoutedEventArgs args)
    {
        string? equation = EquationInput.Text;
        bool isEquationNull = equation is null;
        if (!isEquationNull) 
        {
            equation = equation.ToLower();
        }
        else 
        {
            OperationLog.Text = "ERROR: INCORRECT EQUATION: NullException";
            return;
        }

        bool isBracketsCorrect = calculator.CheckBrackets(equation);
        if (!isBracketsCorrect) 
        {
            OperationLog.Text = "ERROR: INCORRECT EQUATION: BracketsException";
            return;
        }
        //Init variables
        calculator.InitVariables(equation);
        bool isVariablesCorrect = calculator.CheckVariables(equation);
        if (!isVariablesCorrect)
        {
            OperationLog.Text = "ERROR: INCORRECT EQUATION: VariablesException";
            return;
        }

        bool isOperationsCorrect = calculator.CheckOperations(equation);
        if (!isOperationsCorrect) 
        {
            OperationLog.Text = "ERROR: INCORRECT EQUATION: OperationsException";
            return;
        }
        //Init equation
        calculator.InitEquation(equation);
        //Init table which is based on variables and equation
        calculator.InitTruthTable();
        string[,] truthTable = calculator.truthTable;
        //Show answers in app
        PCNFAnswer.Text = calculator.GetPCNF();
        PDNFAnswer.Text = calculator.GetPDNF();
        MDNFAnswer.Text = calculator.GetMDNF();
        TableAnswer.Text = calculator.PrintTable(truthTable);
    }
}
