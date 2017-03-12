var gulp = require('gulp');
var shell = require('gulp-shell');
var runSequence = require('run-sequence');
var gutil = require('gulp-util');
var sftp = require('gulp-sftp');

gutil.log = gutil.noop;

gulp.task('release', function (callback) {
    runSequence(
        'build',
        'publish',
        'restart-service',
        function (error) {
            if (error) {
                console.log(error.message);
            } else {
                console.log('RELEASE FINISHED SUCCESSFULLY');
            }
            callback(error);
        });
});

gulp.task('publish', function () {
    return gulp.src('bin/Debug/netcoreapp2.0/publish/*')
        .pipe(sftp({
            host: '192.168.0.5',
            user: 'publish',
            pass: 'pub123!',
            remotePath: '/var/www/lashes-api'
        }));
});

gulp.task('build', shell.task([
    'dotnet publish'
]));

gulp.task('restart-service', shell.task([
    'plink -load "pi-publish" -l publish -pw pub123!'
]));

