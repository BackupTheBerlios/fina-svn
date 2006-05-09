#include <stdlib.h>
#include <GL/glx.h>

#include "aw.h"
#include "awos.h"

#define EVMASK 
static Display * g_dpy;
static int g_screen;
static XVisualInfo * g_visual;

struct awxHandle {
	Window win;
	GLXContext ctx;
	XVisualInfo * vinfo;
};

static XVisualInfo* chooseVisual() {
	int att[64];
	int * p = att;
	*p++ = GLX_RGBA;
        *p++ = GLX_DOUBLEBUFFER;
	*p++ = GLX_RED_SIZE; *p++ = 1;
        *p++ = GLX_GREEN_SIZE; *p++ = 1;
        *p++ = GLX_BLUE_SIZE; *p++ = 1;
        *p++ = GLX_DEPTH_SIZE; *p++ = 1;
	*p++ = None;
	return glXChooseVisual(g_dpy, g_screen, att);
}

int awosInit() {
	int hasExtensions = 0;
	g_dpy = XOpenDisplay(0);
	g_screen = DefaultScreen(g_dpy);
	return 0 != glXQueryExtension(g_dpy, 0, 0);
}


void awosEnd() {
	XCloseDisplay(g_dpy);
}

void * awosOpen(const char * t, int w, int h) {
        struct awxHandle * ret = NULL;
	struct awxHandle * han = calloc(1, sizeof(*han));
	han->vinfo = chooseVisual( g_dpy, g_screen);
	if (han->vinfo) {
		han->ctx = glXCreateContext(g_dpy, han->vinfo, NULL, True);
		han->win = XCreateSimpleWindow(g_dpy, RootWindow(g_dpy, g_screen),
					       0, 0, w, h, 0, 0, None);
		XSelectInput(g_dpy, han->win, 
			     KeyPressMask | 
			     ButtonPressMask | 
			     ButtonReleaseMask | 
			     PointerMotionMask | 
			     StructureNotifyMask);
		Atom del = XInternAtom(g_dpy, "WM_DELETE_WINDOW", False);
		XSetWMProtocols(g_dpy, han->win, &del, 1);
		if (han->win && han->ctx)
			ret = han;
	}
	if (!ret && han)
		awosClose(han);
	return han;
}

int awosClose(void * vh) {
	struct awxHandle * han = (struct awxHandle *) vh;
	if (han->win) XDestroyWindow(g_dpy, han->win);
	if (han->ctx) glXDestroyContext(g_dpy, han->ctx);
	if (han->vinfo) XFree(han->vinfo);
	free(han);
}

int awosSwapBuffers(void * vh) {
	struct awxHandle * han = (struct awxHandle *) vh;
	glXSwapBuffers(g_dpy, han->win);
	return 1;
}

int awosMakeCurrent(void * vh) {
	struct awxHandle * han = (struct awxHandle *) vh;
	return glXMakeCurrent(g_dpy, han->win, han->ctx);
}

int awosShow(void * vh) {
	struct awxHandle * han = (struct awxHandle *) vh;
	int ret = 0;
	ret |= XMapWindow(g_dpy, han->win);
	ret |= XSync(g_dpy, False);
	return ret;
}

int awosHide(void * vh) {
	struct awxHandle * han = (struct awxHandle *) vh;
	int ret = 0;
	ret |= XUnmapWindow(g_dpy, han->win);
	ret |= XSync(g_dpy, False);
	return ret;
}

int awosSetTitle(void * vh, const char * t) {
	struct awxHandle * han = (struct awxHandle *) vh;
	int ret = 0;
	ret |= XStoreName(g_dpy, han->win, t);
	ret |= XSetIconName(g_dpy, han->win, t);
	return ret;
}

int awosMove(void * vh, int x, int y) {
	struct awxHandle * han = (struct awxHandle *) vh;
	return XMoveWindow(g_dpy, han->win, x, y);
}

int awosResize(void * vh, int w, int h) {
	struct awxHandle * han = (struct awxHandle *) vh;
	return XResizeWindow(g_dpy, han->win, w, h);
}

static int mapButton(int button) {
	int which;
	switch (button) {
	case Button1:
		which = AW_KEY_MOUSELEFT; break;
	case Button2:
		which = AW_KEY_MOUSEMIDDLE; break;
	case Button3:
		which = AW_KEY_MOUSERIGHT; break;
	case Button4:
		which = AW_KEY_MOUSEWHEELUP; break;
	case Button5:
		which = AW_KEY_MOUSEWHEELDOWN; break;
	default:
		which = AW_KEY_NONE;
	}
	return which;
}

int mapKey(KeyCode keycode) {
	return XKeycodeToKeysym(g_dpy, keycode, 0);
}

struct awEvent * awosNextEvent(void * vh) {
	struct awxHandle * han = (struct awxHandle *) vh;
	static struct awEvent ev;
	struct awEvent * pev = NULL;
	XEvent event;
	if (XPending(g_dpy)) { 
		pev = &ev;
		XNextEvent(g_dpy, &event);
		switch(event.type) {
			
		case ClientMessage:
			ev.type = AW_EVENT_CLOSE;
			break;
			
		case ConfigureNotify:
			ev.type = AW_EVENT_RESIZE;
			ev.resize.w = event.xconfigure.width;
			ev.resize.h = event.xconfigure.height;
			break;
			
		case ButtonPress:
			ev.type = AW_EVENT_DOWN;
			ev.down.which = mapButton(event.xbutton.button);
			break;

		case ButtonRelease:
			ev.type = AW_EVENT_UP;
			ev.up.which = mapButton(event.xbutton.button);
			break;

		case MotionNotify:
			ev.type = AW_EVENT_MOTION;
			ev.motion.x = event.xmotion.x;
			ev.motion.y = event.xmotion.y;
			break;

		case KeyPress:
			ev.type = AW_EVENT_DOWN;
			ev.down.which = mapKey(event.xkey.keycode);
			break;

		default:
			ev.type = AW_EVENT_UNKNOWN;
			break;
		}
	}
	return pev;
}
