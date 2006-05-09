
enum {
  AW_EVENT_UNKNOWN,
  AW_EVENT_RESIZE,
  AW_EVENT_CLOSE,
  AW_EVENT_DOWN,
  AW_EVENT_UP,
  AW_EVENT_MOTION,
} eventType;

enum {
  AW_KEY_NONE,
  AW_KEY_MOUSEWHEELUP, 
  AW_KEY_MOUSEWHEELDOWN,
  AW_KEY_MOUSELEFT, 
  AW_KEY_MOUSEMIDDLE,
  AW_KEY_MOUSERIGHT,
};

struct awEvent {
  int type;
  union {
    struct { int w, h; } resize;
    struct { int which; } down;
    struct { int which; } up;
    struct { int x, y; } motion;
  };
};

int awInit();
void awEnd();
int awOpen();
void awSelect(int);
void awClose();
void awSwapBuffers();
void awMakeCurrent();
void awShow();
void awHide();
void awSetTitle(const char *);
void awMove(int, int);
void awResize(int, int);
struct awEvent * awNextEvent();
