#include <stdlib.h>
#include <GL/glx.h>

#include "aw.h"
#include "awos.h"

#define EVMASK 
static Display * g_dpy;
static int g_screen;
static XVisualInfo * g_visual;

struct _aw {
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

aw awosOpen(const char * t, int width, int height) {
	aw ret = NULL;
	aw w = calloc(1, sizeof(*ret));
	w->vinfo = chooseVisual( g_dpy, g_screen);
	if (w->vinfo) {
		Atom del;
		w->ctx = glXCreateContext(g_dpy, w->vinfo, NULL, True);
		w->win = XCreateSimpleWindow(g_dpy, RootWindow(g_dpy, g_screen),
					     0, 0, width, height, 0, 0, None);
		XSelectInput(g_dpy, w->win, 
			     KeyPressMask | 
			     ButtonPressMask | 
			     ButtonReleaseMask | 
			     PointerMotionMask | 
			     StructureNotifyMask);
		del = XInternAtom(g_dpy, "WM_DELETE_WINDOW", False);
		XSetWMProtocols(g_dpy, w->win, &del, 1);
		if (w->win && w->ctx)
			ret = w;
	}
	if (!ret)
		awosClose(w);
	return ret;
}

int awosClose(aw w) {
	if (w) {
		if (w->win) XDestroyWindow(g_dpy, w->win);
		if (w->ctx) glXDestroyContext(g_dpy, w->ctx);
		if (w->vinfo) XFree(w->vinfo);
		free(w);
	}
	return w != 0;
}

int awosSwapBuffers(aw w) {
	glXSwapBuffers(g_dpy, w->win);
	return 1;
}

int awosMakeCurrent(aw w) {
	return glXMakeCurrent(g_dpy, w->win, w->ctx);
}

int awosShow(aw w) {
	int ret = 0;
	ret |= XMapWindow(g_dpy, w->win);
	ret |= XSync(g_dpy, False);
	return ret;
}

int awosHide(aw w) {
	int ret = 0;
	ret |= XUnmapWindow(g_dpy, w->win);
	ret |= XSync(g_dpy, False);
	return ret;
}

int awosSetTitle(aw w, const char * t) {
	int ret = 0;
	ret |= XStoreName(g_dpy, w->win, t);
	ret |= XSetIconName(g_dpy, w->win, t);
	return ret;
}

int awosMove(aw w, int x, int y) {
	return XMoveWindow(g_dpy, w->win, x, y);
}

int awosResize(aw w, int width, int height) {
	return XResizeWindow(g_dpy, w->win, width, height);
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

struct awEvent * awosNextEvent(aw w) {
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
			ev.u.resize.w = event.xconfigure.width;
			ev.u.resize.h = event.xconfigure.height;
			break;
			
		case ButtonPress:
			ev.type = AW_EVENT_DOWN;
			ev.u.down.which = mapButton(event.xbutton.button);
			break;

		case ButtonRelease:
			ev.type = AW_EVENT_UP;
			ev.u.up.which = mapButton(event.xbutton.button);
			break;

		case MotionNotify:
			ev.type = AW_EVENT_MOTION;
			ev.u.motion.x = event.xmotion.x;
			ev.u.motion.y = event.xmotion.y;
			break;

		case KeyPress:
			ev.type = AW_EVENT_DOWN;
			ev.u.down.which = mapKey(event.xkey.keycode);
			break;

		default:
			ev.type = AW_EVENT_UNKNOWN;
			break;
		}
	}
	return pev;
}
