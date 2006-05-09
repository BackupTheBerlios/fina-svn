#include <stdio.h>
#include <stdarg.h>
#include <string.h>
#include "aw.h"
#include "awos.h"
#define MAX_WINDOWS 16
static void * g_handle[MAX_WINDOWS];
static void * g_current;

void report(const char * fmt, ...) {
	va_list ap;
	va_start(ap, fmt);
	fprintf(stderr, "ERROR: ");
	vfprintf(stderr, fmt, ap);
	va_end(ap);
	fprintf(stderr, "\n");
	fflush(stderr);
}

static int freeSlot() {
	unsigned free = 0;
	unsigned i = MAX_WINDOWS;
	while (i-- > 1)
		if (!g_handle[i])
			free = i;
	return free;
}

int awInit() {
	memset(g_handle, 0, sizeof(g_handle));
	g_current = 0;
	return awosInit();
}

void awEnd() {
	awosEnd();
}

void awSelect(int i) {
	g_current = g_handle[i];
}

int awOpen() {
	int slot = freeSlot();
	if (slot) 
		g_handle[slot] = awosOpen("AW Window", 100, 100);
	else {
		report("Too many windows");
		slot = 0;
	}
	if (!g_handle[slot]) {
		report("Unable to open window");
		slot = 0;
	}
	if (slot) {
		awSelect(slot);
		awMakeCurrent();
	}
	return slot;
}

void awClose() {
	if (!awosClose(g_current)) 
		report("Unable to close window");
}

void awSwapBuffers() {
	if (!awosSwapBuffers(g_current))
		report("awSwapBuffers failed");
}

void awMakeCurrent() {
	if (!awosMakeCurrent(g_current))
		report("awMakeCurrent failed");
}

void awShow() {
	if (!awosShow(g_current))
		report("awShow failed");
}

void awHide() {
	if (!awosHide(g_current))
		report("awHide failed");
}

void awSetTitle(const char * t) {
	if (!awosSetTitle(g_current, t))
		report("awSetTitle failed");
}

void awMove(int x, int y) {
	if (!awosMove(g_current, x, y))
		report("awMove failed");
}

void awResize(int w, int h) {
	if (!awosResize(g_current, w, h))
		report("awResize failed");
}

struct awEvent * awNextEvent() {
	struct awEvent * ev = awosNextEvent(g_current);
	report("returning %p", ev);
	return ev;
}
