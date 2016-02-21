﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;
using Dependencies;
using System.IO;

namespace SS
{
    /// <summary>
    /// The cell class holds the contents and values of the cell.
    /// </summary>
    class Cell
    {
        /// <summary>
        /// Holds the base contents of the cell, before it has been evaluted.
        /// Will hold a string, formula, or double.
        /// </summary>
        private object contents = "";

        /// <summary>
        /// Sets the contents of the cell and returns them if needed.
        /// </summary>
        public object Content
        {
            get { return contents; }
            set
            {
                contents = value;
            }
        }
    }
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a cell name if and only if it consists of one or more letters, 
    /// followed by a non-zero digit, followed by zero or more digits.  Cell names
    /// are not case sensitive.
    /// 
    /// For example, "A15", "a15", "XY32", and "BC7" are cell names.  (Note that 
    /// "A15" and "a15" name the same cell.)  On the other hand, "Z", "X07", and 
    /// "hello" are not cell names."
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important, and it is important that you understand the distinction and use
    /// the right term when writing code, writing comments, and asking questions.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In an empty spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
    /// The value of a Formula, of course, can depend on the values of variables.  The value 
    /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
    /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
    /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
    /// is a double, as specified in Formula.Evaluate.
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// A dictionary that pairs a cell's name to the cell that it goes to. 
        /// </summary>
        private Dictionary<string, Cell> cellNames;

        /// <summary>
        /// A dependency graph that keeps track of whhat cells need anothe cell to calculate their value
        /// </summary>
        private DependencyGraph graph;

        /// <summary>
        /// A default constructor that sets up a new empty spreadsheet for the program.
        /// </summary>
        public Spreadsheet()
        {
            cellNames = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
        }

        // ADDED FOR PS6
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get
            {
                throw new NotImplementedException();
            }

            protected set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {

            if (name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
            name = name.ToUpper();
            if (!cellNames.ContainsKey(name))
            {
                cellNames.Add(name, new Cell());
            }

            return cellNames[name].Content;
            

            
        }
        // ADDED FOR PS6
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach(KeyValuePair<string, Cell> pair in cellNames)
            {
                if (!(pair.Value.Content is string) || (string)(pair.Value.Content) != "")
                {
                    yield return pair.Key;
                }
            }
        }

        // ADDED FOR PS6
        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the isvalid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        public override void Save(TextWriter dest)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected  override ISet<string> SetCellContents(string name, Formula formula)
        {

            if (name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
            name = name.ToUpper();
            if (!cellNames.ContainsKey(name))
            {
                cellNames.Add(name, new Cell());
            }

                //if (cellNames[name].Content is Formula)
                //{
                //    Formula form = (Formula)(cellNames[name].Content);
                //    foreach (string variable in form.GetVariables())
                //    {
                        
                //        if (NameValidation(variable))
                //        {
                //        graph.RemoveDependency(variable.ToUpper(), name);
                //        }
                //    }
                //}

            List<string> nameList = new List<string>();
            

            foreach(string variable in formula.GetVariables())
            {
                if (NameValidation(variable))
                {
                    if (!cellNames.ContainsKey(variable.ToUpper()))
                    {
                        cellNames.Add(variable.ToUpper(), new Cell());
                    }
                    nameList.Add(variable);
                    
                }
            }


            HashSet<string> result = new HashSet<string>();
            foreach (string child in GetCellsToRecalculate(name))
            {
                result.Add(child);
            }
            graph.ReplaceDependees(name, nameList);
            cellNames[name].Content = formula;
            return result;
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {

            if (text == null)
            {
                throw new ArgumentNullException(text);
            }
            if (name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
            name = name.ToUpper();
            if (!cellNames.ContainsKey(name))
            {
                cellNames.Add(name, new Cell());
            }

            if (cellNames[name].Content is Formula)
            {
                Formula form = (Formula)(cellNames[name].Content);
                foreach (string variable in form.GetVariables())
                {
                    if (NameValidation(variable))
                    {
                        graph.RemoveDependency(variable.ToUpper(), name);
                    }
                }
            }

            cellNames[name].Content = text;

            HashSet<string> result = new HashSet<string>();
            foreach (string child in GetCellsToRecalculate(name))
            {
                result.Add(child);
            }

            return result;
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {

            if (name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
            name = name.ToUpper();
            if (!cellNames.ContainsKey(name))
            {
                cellNames.Add(name, new Cell());
            }

            if (cellNames[name].Content is Formula)
            {
                Formula form = (Formula)(cellNames[name].Content);
                foreach (string variable in form.GetVariables())
                {
                    if (NameValidation(variable))
                    {
                        graph.RemoveDependency(variable.ToUpper(), name);
                    }
                }
            }

            cellNames[name].Content = number;

            HashSet<string> result = new HashSet<string>();
            foreach(string child in GetCellsToRecalculate(name))
            {
                result.Add(child);
            }

            return result;
        }

        // ADDED FOR PS6
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            double test;
            ISet<string> result;
            
            if (name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
            name = name.ToUpper();
            if (double.TryParse(content, out test)){
                result = SetCellContents(name, test);
            }

            else if(content[0] == '=')
            {
                 result = SetCellContents(name, new Formula(content));
            }

            else
            {
                result = SetCellContents(name, content);
            }
            return result;
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {

            if (name == null)
            {
                throw new ArgumentNullException(name);
            }
            if (!NameValidation(name))
            {
                throw new InvalidNameException();
            }
            name = name.ToUpper();
            foreach (string child in graph.GetDependents(name))
            {
                yield return child;
            }
        }

        
        /// <summary>
        /// A private method that will validate on whether a cell name is a valid name.
        /// If the inputed name is valid
        /// (Starts with a letter, followed by a nonzero digit, and the rest of the characters are digits)
        /// then the method returns true.  Else, the method returns false
        /// Used by the setCellContent method to validate the chosen name, and also to confirm whether
        /// a variable in a formula is a cell name or a variable.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool NameValidation(string name)
        {
            name = name.ToUpper();
            int numTest = 0;
            if (name.Length > 1)
            {
                if (char.IsLetter(name[0]) && (int.TryParse(name[1].ToString(), out numTest) && numTest > 0))
                {
                    for (int i = 2; i < name.Length; i++)
                    {
                        if(!int.TryParse(name[i].ToString(), out numTest))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
