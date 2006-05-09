int awosInit();
void awosEnd();
void * awosOpen(const char *, int, int);
int awosClose(void *);
int awosSwapBuffers(void *);
int awosMakeCurrent(void *);
int awosShow(void *);
int awosHide(void *);
int awosSetTitle(void *, const char *);
int awosMove(void *, int, int);
int awosResize(void *, int, int);
struct awEvent * awosNextEvent(void *);
