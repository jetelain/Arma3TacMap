// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('.btn-copy').on('click', function () {
    var target = $('#'+$(this).attr('data-copy')).get(0);
    target.select();
    document.execCommand("copy");
});