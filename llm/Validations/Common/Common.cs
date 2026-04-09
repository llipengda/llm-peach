using System;
using System.Collections.Generic;
using Peach.Core;
using Peach.Core.Cracker;
using Peach.Core.Dom;
using Peach.Core.IO;
using Peach.Pro.Core;

namespace Peach.LLM.Validations.Common
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class FixerTestAttribute : Attribute
	{
		public string Name { get; private set; }

		public FixerTestAttribute(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Test name is required.", "name");

			Name = name;
		}
	}
    
    public class DataParser
    {
        private readonly string _pitFile;
        private readonly string _dataModelName;
        private readonly Dom _dom;
        private readonly Peach.Core.Dom.DataModel _dataModel;

        public DataParser(string pitFile, string dataModelName)
        {
            if (string.IsNullOrWhiteSpace(pitFile))
                throw new ArgumentException("PIT file path is required.", "pitFile");

            if (string.IsNullOrWhiteSpace(dataModelName))
                throw new ArgumentException("DataModel name is required.", "dataModelName");

            ClassLoader.Initialize("./Plugins");

            _pitFile = pitFile;
            _dataModelName = dataModelName;

            var parser = new ProPitParser(null, null, _pitFile);
            var parserArgs = new Dictionary<string, object>();
            _dom = parser.asParser(parserArgs, _pitFile);

            if (!_dom.dataModels.Contains(_dataModelName))
                throw new ArgumentException(string.Format("Failed to find DataModel '{0}' in Pit File.", _dataModelName), "dataModelName");

            _dataModel = _dom.dataModels[_dataModelName];
        }

        public DataElement Parse(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var dataStream = new BitStream(data);

            var cracker = new DataCracker();
            cracker.CrackData(_dataModel, dataStream);

            return _dataModel;
        }
    }
}
