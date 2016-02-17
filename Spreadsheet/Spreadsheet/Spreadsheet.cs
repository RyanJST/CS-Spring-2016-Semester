using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;
using Dependencies;

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

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if(name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
            if (!cellNames.ContainsKey(name))
            {
                cellNames.Add(name, new Cell());
            }

            return cellNames[name].Content;
            

            
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
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            bool test;
            ISet<string> values;
            if (name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
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
                            graph.RemoveDependency(variable, name);
                        }
                    }
                }
            
            cellNames[name].Content = formula;

            foreach(string variable in formula.GetVariables())
            {
                if (NameValidation(variable))
                {
                    if (!cellNames.ContainsKey(variable))
                    {
                        cellNames.Add(variable, new Cell());
                    }
                    graph.AddDependency(variable, name);
                }
            }

            return getAllDependencies(name);
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
        public override ISet<string> SetCellContents(string name, string text)
        {
            if(text == null)
            {
                throw new ArgumentNullException(text);
            }
            if (name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
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
                        graph.RemoveDependency(variable, name);
                    }
                }
            }

            cellNames[name].Content = text;
            return getAllDependencies(name);
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
        public override ISet<string> SetCellContents(string name, double number)
        {
            if (name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
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
                        graph.RemoveDependency(variable, name);
                    }
                }
            }

            cellNames[name].Content = number;
            return getAllDependencies(name);
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
            if(name == null)
            {
                throw new ArgumentNullException(name);
            }
            if (!NameValidation(name))
            {
                throw new InvalidNameException();
            }
            foreach(string child in graph.GetDependents(name))
            {
                yield return child;
            }
        }

        /// <summary>
        /// this private method finds all dependencies of a cell, direct and indirect.
        /// It will return as a iset, and is used by setCellContents, to return all dependencies
        /// </summary>
        /// <param name="name">The name of the cell to find all dependencies, direct or indirect</param>
        /// <returns></returns>
        private ISet<string> getAllDependencies(string name)
        {
            List<string> current = new List<string>();
            current.Add(name);
            ISet<string> values = new HashSet<string>();
            values.Add(name);
            while(current.Count > 0)
            {
                foreach(string child in graph.GetDependents(current[0]))
                {
                    if (child == name)
                    {
                        throw new CircularException();
                    }
                    if (graph.HasDependents(child))
                    {
                        current.Add(child);
                    }
                    values.Add(child);
                }
                current.RemoveAt(0);
            }
            return values;
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
            int numTest = 0;
            string fullTest = null;
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
