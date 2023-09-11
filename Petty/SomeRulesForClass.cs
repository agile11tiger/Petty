using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Provider.ContactsContract.CommonDataKinds;

namespace Petty
{
    internal class SomeRulesForClass: IDisposable
    {
        public SomeRulesForClass()
        {
            Initialize();
        }

        public SomeRulesForClass(string name)
        {
        }

        public SomeRulesForClass(string arg1, string arg2, string arg3)
        {
        }

        /// <summary>
        /// Doesn't fit on the screen.
        /// </summary>
        public SomeRulesForClass(
            string arg1, 
            string arg2,
            string arg3, 
            string arg8888888888888888888888888888888888888888888888)
        {
        }

        /// <summary>
        /// Only this method if its needed.
        /// </summary>
        public void Initialize()
        {

        }

        /// <summary>
        /// Only finalizer
        /// </summary>
        ~SomeRulesForClass()
        {
            Dispose(disposing: false);
        }

        //-------------------- upper case fields
        private const string CONST_FIELD = "";
        //---------------------------------

        //This applies to all code. Note the order, which depends on the group and length.
        //-------------------------- sort only by length 
        private string _field;
        private string _field888;
        private string _field99999;
        private bool _disposedValue;
        private event TigerHandler _notify;
        private static string _staticField;
        private static string _staticField555;
        private readonly string _readonlyField;
        private static List<string> _staticList;
        private List<string> _list = new List<string>();
        //--------------------------

        public event TigerHandler Notify;
        public delegate void TigerHandler(string message);

        protected string ProtectedProperty { get; set; }
        public string PublicPrivateSetProperty { get; private set; }
        public string PublicProtectedSetProperty { get; protected set; }
        public string PublicProperty { get; set; }
        public static string PublicStaticProperty { get; set; }
        public static string PublicStaticPrivateSetProperty { get; private set; }
        public static string PublicStaticProtectedSetProperty { get; protected set; }

        //-----------------------------complex data structures
        public List<string> PublicList { get; set; }
        public Dictionary<string, string> PublicDictionary { get; set; }
        //------------------------------

        //-----------------------------Property with summary
        /// <summary>
        /// There must be one blank line between properties with summary.
        /// </summary>
        public string PublicPropertyWithSummary { get; set; }

        /// <summary>
        /// There must be one blank line between properties with summary.
        /// </summary>
        public static string PublicStaticPropertyWithSummary { get; set; }
        //------------------------------

        public static void SomeStaticMethod()
        {
        }

        public void SomePublicMethod()
        {
            var tiger = string.Empty;
            tiger = string.Empty;

            if (true)
                tiger = "Tiger";

            //This is not always possible, for example if a private method that should be under public methods calls a public method
            SomeMethodThatWasCalledByAnotherMethodMustBeBelowIt();
            tiger = "Tiger";
        }

        public void SomeMethodThatWasCalledByAnotherMethodMustBeBelowIt()
        {
            var tiger = string.Empty;

            if (true)
            {
                tiger = "Tiger";
                tiger = "Tiger";
            }

            tiger = "Tiger";
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        private void SomePrivateMethod()
        {
        }
    }
}
