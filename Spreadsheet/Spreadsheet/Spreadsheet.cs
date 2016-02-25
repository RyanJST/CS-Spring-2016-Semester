using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Formulas;
using Dependencies;
using System.IO;
using System.Xml;
using System.Xml.Schema;

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
        /// Holds the calculated value of the contents of the cell after evalution.
        /// Will hold a string, double, or Formula Error
        /// </summary>
        private object values = "";

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

        /// <summary>
        /// Returns the value of the current cell
        /// </summary>
        public object Value
        {
            get
            {
                return values;
            }
        }

        /// <summary>
        /// Recalculates the value of the current cell
        /// </summary>
        /// <param name="lookup"></param>
        public void changeValues(Dictionary<string, Cell> lookup)
        {
            if(contents is Formula)
            {
                Formula form = (Formula)contents;
                try {
                    values = form.Evaluate(s => lookupMethod(lookup, s));
                }
                catch (Exception e)
                {
                    if (e is CircularException)
                    {
                        values = new FormulaError("Created Circular Dependency");
                    }
                    else if(e is FormulaFormatException)
                    {
                        values = new FormulaError("Incorrect formatting of formula.");
                    }
                    else if (e is FormulaEvaluationException)
                    {
                        values = new FormulaError("Error evaluting formula");
                    }
                }
            }

            else
            {
                values = contents;
            }
        }
        
        /// <summary>
        /// Looks up the value of other cells when needed.
        /// </summary>
        /// <param name="cellTable"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private double lookupMethod(Dictionary<string, Cell> cellTable, string name)
        {
            if(!(cellTable[name].values is double) && !(cellTable[name].values is Formula))
            {
                throw new UndefinedVariableException(name);
            }

            return (double)cellTable[name].values;
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

        private Regex IsValid;

        private Regex nameValid;

        private bool change;

        /// <summary>
        /// A default constructor that sets up a new empty spreadsheet for the program.
        /// </summary>
        public Spreadsheet()
        {
            nameValid = new Regex(@"^[A-Z]+[1-9][0-9]*$");
            IsValid = new Regex("^.*$");
            cellNames = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
            change = false;
        }

        /// <summary>
        /// Create a spreadsheet that checkks any potential cell names against 
        /// </summary>
        /// <param name="isValid"></param>
        public Spreadsheet(Regex isValid)
        {
            nameValid = new Regex(@"^[A-Z]+[1-9][0-9]*$");
            IsValid = isValid;
            cellNames = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
            change = false;
        }

        public Spreadsheet(TextReader source)
        {
            XmlSchemaSet sc = new XmlSchemaSet();
            XmlTextReader scheme = new XmlTextReader("Spreadsheet.xsd");
            XmlSchema myschema = XmlSchema.Read(scheme, ValidationCallback);
            sc.Add(myschema);
            TextReader test = source;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.ValidationEventHandler += ValidationCallback;
            nameValid = new Regex(@"^[A-Z]+[1-9][0-9]*$");
            cellNames = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
            change = false;
            try {
                using (XmlReader reader = XmlReader.Create(source, settings))
                {
                    while (reader.Read())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                if (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    IsValid = new Regex(reader["IsValid"]);
                                }
                                break;
                            case "cell":
                                SetContentsOfCell(reader["name"], reader["contents"]);

                                break;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                throw new IOException();
            }
            foreach(string cell in GetNamesOfAllNonemptyCells())
            {
                if (cellNames[cell].Value is FormulaError)
                {
                    throw new SpreadsheetReadException("Formula Error ");
                }
            }
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
                return change;
            }

            protected set
            {
                change = value;
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
            if (name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
            name = name.ToUpper();
            if (!cellNames.ContainsKey(name))
            {
                cellNames.Add(name, new Cell());
            }

            return cellNames[name].Value;
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
            try {
                using (XmlWriter writer = XmlWriter.Create(dest))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("IsValid", IsValid.ToString());

                    foreach (string cellName in GetNamesOfAllNonemptyCells())
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteAttributeString("name", cellName);
                        if (cellNames[cellName].Content is Formula)
                        {
                            writer.WriteAttributeString("contents", "=" + cellNames[cellName].Content.ToString());
                        }
                        else
                        {
                            writer.WriteAttributeString("contents", cellNames[cellName].Content.ToString());
                        }
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                Changed = false;
            }
            catch
            {
                throw new IOException();
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
        protected  override ISet<string> SetCellContents(string name, Formula formula)
        {

            if (!cellNames.ContainsKey(name))
            {
                cellNames.Add(name, new Cell());
            }
            foreach(string variable in formula.GetVariables())
            {
                    if (!cellNames.ContainsKey(variable))
                    {
                        cellNames.Add(variable, new Cell());
                    }
            }
            graph.ReplaceDependees(name, formula.GetVariables());
            ISet<string> result = new HashSet<string>(GetCellsToRecalculate(name));
            
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

            
            return new HashSet<String>(GetCellsToRecalculate(name));
            
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

            return new HashSet<string>(GetCellsToRecalculate(name));
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

            if(content == null)
            {
                throw new ArgumentNullException();
            }
            name = name.ToUpper();
            if (double.TryParse(content, out test)){
                result = SetCellContents(name, test);
            }

            else if(content.Length >=1 && content[0] == '=')
            {
                string edited = content.Remove(0,1);
                
                result = SetCellContents(name, new Formula(edited, s => s.ToUpper(), s => nameValid.IsMatch(s)));
            }

            else
            {
                result = SetCellContents(name, content);
            }

            foreach (string recalc in result)
            {
                cellNames[recalc].changeValues(cellNames);
            }
            Changed = true;
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
            if(!IsValid.IsMatch(name) || !nameValid.IsMatch(name))
            {
                return false;
            }
            return true;
        }

        private static void ValidationCallback(object sender, ValidationEventArgs e)
        {
            throw new SpreadsheetReadException("Error Reading Spreadsheet");
        }
    }
}
