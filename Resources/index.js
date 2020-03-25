﻿(function ($) {

    //$.page('subject/edit').tpl('subject', 'subject/view', function (root, title) {

    //    var t = new WDK.POS.Pager(root);
    //    t.model = "Subject";
    //    t.cmd = 'UIData'
    //    root.on('hash', function (e, v) {
    //        if (v.key) {
    //            t.search = { Id: v.key };
    //            WDK.POS.Command(t.model, t.cmd, WDK.extend({
    //                limit: 30
    //            }, t.search), function (xhr) {
    //                var tt = xhr.Title;
    //                if (tt) {
    //                    title.text(tt.title || tt.text);
    //                }
    //                t.b.html('');
    //                t.dataSource(xhr);
    //            });

    //        }
    //    });
    //})
    $.tpl('link', 'link/link', function (root, title) {
        var isOpen = false;
        root.on('hash', function (e, v) {
            v.key ? WDK.POS.Command('Account', 'Link', v.key) : 0;
        });
        root.ui('Link', function (e, v) {
            if (!isOpen) {
                title.text(v.link.Caption);
                root.find('iframe').attr('src', v.link.Url);
                isOpen = true;
            }
        })
    });
})(WDK)