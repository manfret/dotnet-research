namespace dotnet7_research.Meetup;

public static class LambdaDemo
{
    public static void M1()
    {
        Action<string> action = delegate (string message)
        {
            Console.WriteLine($"In delegate: {message}");
        };
        action("Message");

        action = (string message) =>
        {
            Console.WriteLine($"In lambda: {message}");
        };
        action("Message");

        action = (string message) => Console.WriteLine($"In lambda: {message}");
        action("Message");

        action = message => Console.WriteLine($"In lambda: {message}");
        action("Message");
    }

    public static void M2()
    {
        Func<int, int, int> multiply = (int x, int y) => { return x * y; };
        multiply = (int x, int y) => x * y;
        multiply = (x, y) => x * y;

        var res = multiply(2, 5);
        Console.WriteLine($"Multiply res = {res}");
    }

    public static void M3()
    {
        Func<string, int> squareLength = (string text) =>
        {
            var length = text.Length;
            return length * length;
        };
        squareLength = text =>
        {
            var length = text.Length;
            return length * length;
        };
        var res = squareLength("my text");
        Console.WriteLine($"Squared length = {res}");
    }

    private class CapturedVariablesDemo
    {
        private readonly string _instanceField = "instance field";

        public Action<string> CreateAction(string methodParameter)
        {
            string methodLocal = "method local";
            string uncaptured = "uncaptured";

            //no capturing variables -> static method
            //only capturing is instance -> instance method
            //has capturing variables -> private class to capture context
            Action<string> action = lambdaParameter =>
            {
                string lambdaLocal = "lambda local";
                Console.WriteLine($"Instance field: {_instanceField}");
                Console.WriteLine($"Method parameter: {methodParameter}");
                Console.WriteLine($"Method local: {methodLocal}");
                Console.WriteLine($"Lambda parameter: {lambdaParameter}");
                Console.WriteLine($"Lambda local: {lambdaLocal}");
            };
            methodLocal = "modified method local";
            return action;
        }



        private class LambdaContext
        {
            public CapturedVariablesDemo _originalThis;
            public string _methodParameter;
            public string _methodLocal;

            public void Method(string lambdaParameter)
            {
                string lambdaLocal = "lambda local";
                Console.WriteLine($"Instance field: {_originalThis._instanceField}");
                Console.WriteLine($"Method parameter: {_methodParameter}");
                Console.WriteLine($"Method local: {_methodLocal}");
                Console.WriteLine($"Lambda parameter: {lambdaParameter}");
                Console.WriteLine($"Lambda local: {lambdaLocal}");
            }
        }

        public Action<string> CreateActionDec(string methodParameter)
        {
            var context = new LambdaContext();
            context._originalThis = this;
            context._methodParameter = methodParameter;
            context._methodLocal = "method local";
            string uncaptured = "uncaptured";

            Action<string> action = context.Method;
            context._methodLocal = "modified method local";
            return action;
        }
    }

    public static void M4()
    {
        var demo = new CapturedVariablesDemo();
        var methodArgument = "method argument";
        var action = demo.CreateAction(methodArgument);
        methodArgument = "method argument modified";
        action("lambda argument");
    }

    public static void M5()
    {
        var demo = new CapturedVariablesDemo();
        var methodArgument = "method argument";
        var action = demo.CreateActionDec(methodArgument);
        methodArgument = "method argument modified";
        action("lambda argument");
    }

    private class VariableCapturingInLoop
    {
        public List<Action> CreateActions()
        {
            List<Action> actions = new List<Action>();
            for (int i = 0; i < 5; i++)
            {
                var text = $"message {i}";
                //var text = $"{i}";
                //var text = string.Format("message {0}", i);
                actions.Add(() => Console.WriteLine(text));
                //actions.Add(() => Console.WriteLine(i));
                //text = $"message modified {i}";
            }
            return actions;
        }

        private class LambdaContext
        {
            public string _text;

            public void Method()
            {
                Console.WriteLine(_text);
            }
        }

        public List<Action> CreateActionsDec()
        {
            List<Action> actions = new List<Action>();
            for (int i = 0; i < 5; i++)
            {
                var context = new LambdaContext();
                context._text = $"message {i}";
                actions.Add(context.Method);
            }
            return actions;
        }
    }

    public static void M6()
    {
        var demo = new VariableCapturingInLoop();
        var actions = demo.CreateActions();
        foreach (var action in actions)
        {
            action();
        }
    }

    public static void M7()
    {
        var demo = new VariableCapturingInLoop();
        var actions = demo.CreateActionsDec();
        foreach (var action in actions)
        {
            action();
        }
    }

    private class VariableCapturingInnerOuter
    {
        public List<Action> CreateCountingActions()
        {
            List<Action> actions = new List<Action>();
            int outerCounter = 0;
            for (int i = 0; i < 2; i++)
            {
                int innerCounter = 0;
                Action action = () =>
                {
                    Console.WriteLine("Outer: {0}; Inner: {1}", outerCounter, innerCounter);
                    outerCounter++;
                    innerCounter++;
                };
                actions.Add(action);
            }
            return actions;
        }

        private class OuterContext
        {
            public int _outerCounter;
        }

        private class InnerContext
        {
            public OuterContext _outerContext;
            public int _innerCounter;

            public void Method()
            {
                Console.WriteLine("Outer: {0}; Inner: {1}", _outerContext._outerCounter, _innerCounter);
                _outerContext._outerCounter++;
                _innerCounter++;
            }
        }

        public List<Action> CreateCountingActionsDec()
        {
            List<Action> actions = new List<Action>();
            OuterContext outerContext = new OuterContext();
            outerContext._outerCounter = 0;
            for (int i = 0; i < 2; i++)
            {
                InnerContext innerContext = new InnerContext();
                innerContext._outerContext = outerContext;
                innerContext._innerCounter = 0;
                Action action = innerContext.Method;
                actions.Add(action);
            }
            return actions;
        }
    }

    public static void M8()
    {
        var demo = new VariableCapturingInnerOuter();
        var actions = demo.CreateCountingActions();
        actions[0]();
        actions[0]();
        actions[0]();
        actions[1]();
        actions[1]();
        actions[1]();
    }

    public static void M9()
    {
        var demo = new VariableCapturingInnerOuter();
        var actions = demo.CreateCountingActionsDec();
        actions[0]();
        actions[0]();
        actions[0]();
        actions[1]();
        actions[1]();
        actions[1]();
    }
}