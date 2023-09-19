using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Provider.ContactsContract.CommonDataKinds;

namespace Petty
{
    /*All errors are caught and logged, the application should work in any case, even if some of its components are unavailable.*/
    //all variables, parameters, fields, properties, methods and etc sort BY GROUPS(all groups i will show below) then BY LENGTH
    internal class SomeRulesForClass: IDisposable
    {
        public SomeRulesForClass()
        {
            Initialize();
            CatNotify += OnUpdate;
        }

        public SomeRulesForClass(string name)
        {
        }

        public SomeRulesForClass(string arg1, string arg2, string arg3)
        {
        }

        /// <summary>
        /// should be move down if doesn't fit on the screen, like this
        /// </summary>
        public SomeRulesForClass(
            string arg1, 
            string arg2,
            string arg3, 
            string arg8888888888888888888888888888888888888888888888)
        {
        }

        /// <summary>
        /// Only this method with this name if its needed. Modifiers doesn`t matter, allowed any.
        /// </summary>
        private void Initialize() //or this InitializeAsync
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

        //-------------------------- private fields
        private string _field;
        private string _field888;
        private string _field99999;
        private bool _isDisposedValue; //all bool variables, parameters, fields, properties with prefix "is"
        private event TigerHandler _notify;
        private static string _staticField;
        private static string _staticField555;
        private readonly string _readonlyField;
        private static List<string> _staticList;
        private List<string> _list = new List<string>();
        //--------------------------

        //-------------------------- events, delegates
        public event CatHandler CatNotify;
        public event TigerHandler TigerNotify;
        public delegate void CatHandler(string message);
        public delegate void TigerHandler(string message);
        //-------------------------- 

        //-------------------------- properties
        public string PublicProperty { get; set; }
        public static bool PublicStaticProperty { get; set; }
        public int PublicPrivateSetProperty { get; private set; }
        public static string PublicStaticProtectedSetProperty { get; protected set; }
        //----------------------------------------------------------------------------

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
        }

        //any PascalCase name with prefix "On"
        public void OnUpdate(string message)
        {
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _isDisposedValue = true;
            }
        }

        private void SomePrivateMethod()
        {
        }
    }
}
