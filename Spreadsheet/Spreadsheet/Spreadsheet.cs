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

    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, Cell> cellNames;
        private DependencyGraph graph;
        public Spreadsheet()
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

            cellNames[name].Content = formula;

            values = getAllDependencies(name, out test);

            if (!test)
            {
                throw new CircularException();
            }

            foreach(string variable in formula.GetVariables())
            {
                graph.AddDependency(name, variable);
            }
            return values;
        }

        public override ISet<string> SetCellContents(string name, string text)
        {
            bool test;
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
            return getAllDependencies(name, out test);
        }

        public override ISet<string> SetCellContents(string name, double number)
        {
            ISet<string> values = null;
            bool test;
            if (name == null || !NameValidation(name))
            {
                throw new InvalidNameException();
            }
            if (!cellNames.ContainsKey(name))
            {
                cellNames.Add(name, new Cell());
            }

            cellNames[name].Content = number;
            return getAllDependencies(name, out test);
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

        private ISet<string> getAllDependencies(string name, out bool test)
        {
            List<string> current = new List<string>();
            current.Add(name);
            ISet<string> values = new HashSet<string>();
            values.Add(name);
            test = true;
            while(current.Count > 0)
            {
                foreach(string child in graph.GetDependents(current[0]))
                {
                    if (values.Contains(child))
                    {
                        test = false;
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
