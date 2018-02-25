/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    fs = require("fs"),
    less = require('gulp-less'),
    path = require('path'),
    using = require('gulp-using');

var paths = {
  webroot: "wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "**/*.css";
paths.less = path.webroot + "**/*.less";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";
paths.lessToCssDest = paths.webroot + "css";

gulp.task("clean:js", function (cb) {
  rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
  rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", gulp.parallel("clean:js", "clean:css"));

gulp.task("min:js", function () {
  return gulp.src(['./wwwroot/css/**/*.js', "!" + paths.minJs], { base: "." })
    .pipe(concat(paths.concatJsDest))
    .pipe(uglify())
    .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
  return gulp.src(['./wwwroot/css/**/*.css', "!" + paths.minCss])
    .pipe(using({}))
    .pipe(concat(paths.concatCssDest))
    .pipe(cssmin())
    .pipe(gulp.dest("."));
});

 
gulp.task('less', function () {
console.log("Test");
  return gulp.src(['./wwwroot/css/**/*.less'], { base: "." })
    .pipe(using({}))
    .pipe(less())
    .pipe(gulp.dest("."));
});

gulp.task("min", gulp.parallel("min:js", "min:css"));

gulp.task("build", gulp.series("clean", "less", "min" ));