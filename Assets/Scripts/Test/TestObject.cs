using UnityEngine;

namespace Test
{
    public class TestObject : MonoBehaviour
    {
        public void Print(string message)
        {
            Debug.Log("Hello" + message + " My Name is" + name);
        }

        public void PrintSpecial(string message)
        {
            Debug.Log("Yo Events are pretty cool " + message + " don't you think?");
        }
    }
}
