#include "fina.h"

int main(int argc, char ** argv)
{
    if (0 == FINA_Init())
        while (0 == FINA_Tick()) 
            ;
    FINA_End();
    return 0;
}

