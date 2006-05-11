int awosInit();
void awosEnd();
aw awosOpen(const char *, int, int);
int awosClose(aw);
int awosSwapBuffers(aw);
int awosMakeCurrent(aw);
int awosShow(aw);
int awosHide(aw);
int awosSetTitle(aw, const char *);
int awosMove(aw, int, int);
int awosResize(aw, int, int);
struct awEvent * awosNextEvent(aw);