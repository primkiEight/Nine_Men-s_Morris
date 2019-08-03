using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathFunctions {

    public static int Modulo(int variable, int lenght)
    {
        int result = variable % lenght;

        if (result < 0)
            result += lenght;

        return result;
    }

}
