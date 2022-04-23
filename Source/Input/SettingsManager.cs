using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace SpaceMarines_TD.Source.Input
{
    internal class SettingsManager
    {
        private const string BindingFileName = @"Controls.json";

        public event EventHandler SettingsChanged;

        public KeyBindings Bindings { get; set; }

        public void Load()
        {
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (storage.FileExists(BindingFileName))
            {
                using (var file = new StreamReader(storage.OpenFile(BindingFileName, FileMode.Open)))
                {
                    var text = file.ReadToEnd();
                    Bindings = JsonConvert.DeserializeObject<KeyBindings>(text);
                }
            }
            else
            {
                Bindings = KeyBindings.Default;
            }

            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Store()
        {
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            var text = JsonConvert.SerializeObject(Bindings);

            using (var file = new StreamWriter(storage.CreateFile(BindingFileName)))
            {
                file.Write(text);
            }

            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
