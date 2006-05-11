#include <stdio.h>
#include <stdarg.h>
#include <string.h>
#include "aw.h"
#include "awos.h"

void report(const char * fmt, ...) {
	va_list ap;
	va_start(ap, fmt);
	fprintf(stderr, "ERROR: ");
	vfprintf(stderr, fmt, ap);
	va_end(ap);
	fprintf(stderr, "\n");
	fflush(stderr);
}

int awInit() {
	return awosInit();
}

void awEnd() {
	awosEnd();
}

aw awOpen() {
	aw w = awosOpen("AW Window", 100, 100);
	if (!w)
		report("Unable to open window");
	return w;
}

void awClose(aw w) {
	if (!awosClose(w)) 
		report("Unable to close window");
}

void awSwapBuffers(aw w) {
	if (!awosSwapBuffers(w))
		report("awSwapBuffers failed");
}

void awMakeCurrent(aw w) {
	if (!awosMakeCurrent(w))
		report("awMakeCurrent failed");
}

void awShow(aw w) {
	if (!awosShow(w))
		report("awShow failed");
}

void awHide(aw w) {
	if (!awosHide(w))
		report("awHide failed");
}

void awSetTitle(aw w, const char * t) {
	if (!awosSetTitle(w, t))
		report("awSetTitle failed");
}

void awMove(aw w, int x, int y) {
	if (!awosMove(w, x, y))
		report("awMove failed");
}

void awResize(aw w, int width, int height) {
	if (!awosResize(w, width, height))
		report("awResize failed");
}

struct awEvent * awNextEvent(aw w) {
	struct awEvent * ev = awosNextEvent(w);
	return ev;
}
