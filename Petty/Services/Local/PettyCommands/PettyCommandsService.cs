using Android.OS;
using Newtonsoft.Json.Bson;
using Petty.Services.Local.PettyCommands.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Local.PettyCommands
{
    public class PettyCommandsService
    {
        public PettyCommandsService(PettyVoiceService pettyVoiceService)
        {
            _pettyVoiceService = pettyVoiceService;
            var iPettyCommandType = typeof(IPettyCommand);
            var currentAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var currentAssembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.FullName.StartsWith(currentAssemblyName));
            var types = currentAssembly.GetTypes().Where(p => iPettyCommandType.IsAssignableFrom(p) && p.IsClass);

            foreach (var type in types)
            {
                var command = (IPettyCommand)Activator.CreateInstance(type);
                _pettyCommands[command.Name] = command;
            }
        }

        private readonly PettyVoiceService _pettyVoiceService;
        private readonly Dictionary<string, IPettyCommand> _pettyCommands = new();

        public void ProcessRawData(byte[] data)
        {
            var command = RecognizeCommand(data);

            if (command == null || !command.TryExecute())
                _pettyVoiceService.PlayCommandExecutionFailed(command);

            _pettyVoiceService.PlayCommandExecutionSuccessed(command);
        }
        
        private IPettyCommand RecognizeCommand(byte[] data)
        {
            var text = GetText(data);

            if (_pettyCommands.TryGetValue(text, out IPettyCommand pettyCommand))
                return pettyCommand;

            foreach (var command in _pettyCommands.Values)
                if (command.CheckComplianceCommand(text))
                    return command;

            return null;
        }

        private string GetText(byte[] data)
        {
            var text = Encoding.UTF8.GetString(data).TrimEnd(new char[] { (char)0 });
            var text11 = Encoding.UTF32.GetString(data).TrimEnd(new char[] { (char)0 });
            var text111 = Encoding.ASCII.GetString(data).TrimEnd(new char[] { (char)0 });
            var text1 = Convert.ToBase64String(data).TrimEnd(new char[] { (char)0 });
            var text2 = BitConverter.ToString(data).TrimEnd(new char[] { (char)0 });
            return text;
        }
    }
}
