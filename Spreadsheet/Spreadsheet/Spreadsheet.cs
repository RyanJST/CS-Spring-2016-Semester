using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;
using Dependencies;

namespace SS
{
    class Cell
    {
        private object contents;

        private object value;

        public object Content
        {
            get { return contents; }
            set
            {
                contents = value;
            }
        }

    }

    class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, Cell> cellNames;
        private DependencyGraph graph;
        Spreadsheet()
        {
            cellNames = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
        }

        public override object GetCellContents(string name)
        {
            if(name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }

            return cellNames[name].Content;
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach(KeyValuePair<string, Cell> pair in cellNames)
            {
                yield return pair.Key;
            }
        }

        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            ISet<string> values = null;
            if (name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
            if (!cellNames.ContainsKey(name))
            {
                cellNames.Add(name, new Cell());
            }

            cellNames[name].Content = formula;
            throw new NotImplementedException();
        }

        public override ISet<string> SetCellContents(string name, string text)
        {
            ISet<string> values = new HashSet<string>();

            if(text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
            if (!cellNames.ContainsKey(name))
            {
                cellNames.Add(name, new Cell());
            }

            cellNames[name].Content = text;
            values.Add(name);
            string current = name;
            
            return values;
        }

        public override ISet<string> SetCellContents(string name, double number)
        {
            ISet<string> values = null;
            if (name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
            if (!cellNames.ContainsKey(name))
            {
                cellNames.Add(name, new Cell());
            }

            cellNames[name].Content = number;
            throw new NotImplementedException();
        }

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

        private ISet<string> getAllDependencies(string name)
        {
            string current = name;

            while (graph.HasDependents(current))
            {
                foreach()
            }
        }

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
                        fullTest += name[i].ToString();
                    }
                    if (fullTest == null || int.TryParse(fullTest, out numTest))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
