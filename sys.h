void Sys_Init(int argc, char ** argv);
void Sys_End();
unsigned Sys_Argc();
char * Sys_Argv(unsigned i);

// File support
void * Sys_FileOpen(const char * name, unsigned mode);
void Sys_FileClose(void * handle);
unsigned Sys_FileThrow();
unsigned Sys_FileRead(void * handle, char * buf, unsigned len);
void Sys_FileWrite(void * handle, char * buf, unsigned len);
void * Sys_FileMMap(void * handle);
unsigned long long Sys_FileSize(void * handle);
unsigned long long Sys_FileTell(void * handle);
void Sys_FileSeek(void * handle, unsigned long long pos);
unsigned Sys_FileLine(void * handle, char * buf, unsigned len);


// Memory
void Sys_MemMove(char * to, const char * from, unsigned bytes);
void Sys_MemSet(char * dst, unsigned c, unsigned bytes);

// Console
unsigned Sys_HasChar();
unsigned Sys_GetChar();
void Sys_PutChar(unsigned c);
