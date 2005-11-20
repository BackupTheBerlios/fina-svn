
                PRIM(ALLOCATE, 250);
                tos = (CELL)Sys_MemAllocate(tos);
                PUSH;
                tos = Sys_Throw();
                NEXT;

                PRIM(FREE, 251);
		tos = Sys_MemFree((void*)tos);
                NEXT;

                PRIM(RESIZE, 252);
                dsp[0] = (CELL)Sys_MemResize((void*)dsp[0], tos);
                tos = Sys_Throw();
                NEXT;
