var gulp = require('gulp');
var sass = require('gulp-sass');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify-es').default;
var sourceMaps = require('gulp-sourcemaps');
var autoprefixer = require('gulp-autoprefixer');
var path = require('path');
var folders = require('gulp-folders-4x');
var pathToFolder = 'src/js';
var cleanCss = require('gulp-clean-css');
var rename = require('gulp-rename');
var javascriptObfuscator = require('gulp-javascript-obfuscator');

var sources = {
    scss: 'src/scss/*.scss',
    js: 'src/js'
},
    dests = {
        css: 'wwwroot/css',
        js: 'wwwroot/js'
    },
    buildWatch = {
        scss: 'src/scss/**/*.scss',
        js: 'src/js/**/*.js'
    },
    watchs = {
        scss: 'css/**/*.css',
        js: 'dest/js/**/*.js',
    },
    autoprefixerOptions = {
        browsers: ['last 2 versions', '> 5%', 'Firefox ESR']
    };

function css() {
    return gulp
        .src(sources.scss)
        .pipe(sourceMaps.init())
        .pipe(sass())
        .on('error', handleError)
        .pipe(autoprefixer(autoprefixerOptions))
        .pipe(sourceMaps.write())
        .pipe(gulp.dest(dests.css))
        .pipe(rename({ extname: '.min.css' }))
        .pipe(cleanCss({ level: 2 }))
        .pipe(gulp.dest(dests.css));
}

function handleError(err) {
    console.log(err.toString());
    this.emit('end');
}

gulp.task('js', folders(pathToFolder, function (folder) {
    return gulp.src(path.join(pathToFolder, folder, '*.js'))
        .pipe(concat(folder + '.js'))
        .pipe(sourceMaps.init()).on('error', handleError)
        .pipe(concat(folder + '.js')).on('error', handleError)
        .pipe(sourceMaps.write()).on('error', handleError)
        .pipe(gulp.dest(dests.js)).on('error', handleError)
        .pipe(rename({ extname: '.min.js' })).on('error', handleError)
        .pipe(javascriptObfuscator())
        .pipe(uglify()).on('error', handleError)
        .pipe(gulp.dest(dests.js));
}));

function watch() {
    gulp.watch(buildWatch.scss, gulp.series('css'));
    gulp.watch(buildWatch.js, gulp.series('js'));
};

exports.css = css;
exports.watch = watch;
exports.default = watch;
