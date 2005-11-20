
#if defined(HAS_FILES)
                PRIM(OPENF, 200);
                dsp[1] = (CELL)Sys_FileOpen(zstr((char*)dsp[1], dsp[0]), tos);
                dsp++;
                tos = Sys_Throw();
                NEXT;

                PRIM(CLOSEF, 201);
                Sys_FileClose((void*)tos);
                tos = Sys_Throw();
                NEXT;

                PRIM(READF, 202);
                dsp[1] = Sys_FileRead((void*)tos, (char*)dsp[1], dsp[0]);
                dsp++;
                tos = Sys_Throw();
                NEXT;
                
                PRIM(WRITEF, 203);
                Sys_FileWrite((void*)tos, (char*)dsp[1], dsp[0]);
                tos = Sys_Throw();
                dsp += 2;
                NEXT;

                PRIM(MMAPF, 204);
                tos = (CELL)Sys_FileMMap((void*)tos);
                PUSH;
                tos = Sys_Throw();
                NEXT;

                PRIM(SEEKF, 205);
                t0 = tos;
                POP;
                POPULL;
                Sys_FileSeek((void*)t0, ull);
                PUSH;
                tos = Sys_Throw();
                NEXT;
                
                PRIM(SIZEF, 206);
                t0 = tos;
                POP;
                ull = Sys_FileSize((void*)t0);
                PUSHULL;
                PUSH;
                tos = Sys_Throw();
                NEXT;

                PRIM(TELLF, 207);
                ull = Sys_FileTell((void*)tos);
                POP;
                PUSHULL;
                PUSH;
                tos = Sys_Throw();
                NEXT;
                        
                PRIM(LINEF, 208);
                dsp[1] = Sys_FileLine((void*)tos, (char*)dsp[1], dsp[0]);
                tos = Sys_Throw();
                dsp[0] = FLAG(tos != -39);
                tos = tos == -39? 0 : tos;
                NEXT;

                PRIM(DELETEF, 209);
                Sys_FileDelete(zstr((char*)*dsp++, tos));
                tos = Sys_Throw();
                NEXT;

                PRIM(STATF, 210);
                dsp[0] = Sys_FileStat(zstr((char*)dsp[0], tos));
                tos = Sys_Throw();
                NEXT;

                PRIM(RENF, 211);
                tos = (CELL)zstr2((char*)dsp[0], tos);
                Sys_FileRen(zstr((char*)dsp[2], dsp[1]), (char*)tos);
                tos = Sys_Throw();
                dsp += 3;
                NEXT;

                PRIM(TRUNCF, 212);
                t0 = tos;
                POP;
                POPULL;
                PUSH;
                Sys_FileTrunc((void*)t0, ull);
                tos = Sys_Throw();
                NEXT;

                PRIM(FLUSHF, 213);
                Sys_FileFlush((void*)tos);
                tos = Sys_Throw();
                NEXT;
                
#endif  // HASFILES

