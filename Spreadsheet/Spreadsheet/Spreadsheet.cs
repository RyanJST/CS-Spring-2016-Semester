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
        private object contents = "";

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
            if (!cellNames.ContainsKey(name))
            {
                cellNames.Add(name, new Cell());
            }

            return cellNames[name].Content;
            

            
        }

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
