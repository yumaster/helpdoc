layui.use(['layer', 'element'], function () {
    var layer = layui.layer,
        element = layui.element,
        e = {
            nav: $('.hl-nav'),
            parse: $('.hl-parse')
        },
        f = {
            prep: function (res) {
                if (res.status == 0) {
                    if (res.result.length > 0) {
                        return res.result;
                    }
                    else layer.msg('暂无任何目录');
                }
                else if (res.status == 4) {
                    layer.msg('没有操作权限');
                }
                else {
                    if (typeof res.status == 'undefined' || typeof res.result == 'undefined') {
                        layer.msg('请求响应错误', { icon: 2 });
                    }
                    else {
                        layer.msg(res.result);
                    }
                }
            }
        };
    $.fn.zTree.init($("#hltree"), {
        data: { key: { isParent: 'i', name: 'n' } },
        async: {
            enable: true,
            type: 'get',
            url: '/home/gettree',
            autoParam: ['id'],
            dataFilter: function (ti, pn, res) {
                return f.prep(res);
            }
        },
        view: { selectedMulti: false },
        callback: {
            onClick: function (eve, ti, tn) {
                if (!tn.i) {
                    //展示面包屑
                    e.nav.empty();
                    $.each(tn.getPath(), function (ti, o) {
                        e.nav.append('<a><cite>' + o.n + '</cite></a>');
                    });
                    element.render('breadcrumb');

                    e.parse.empty();
                    if (tn.u != null) {
                        //e.parse.load(tn.u, function () {
                        //    uParse('.hl-parse', {
                        //        rootPath: '/scripts/ueditor/'
                        //    });
                        //});
                        e.parse.append(tn.u);
                    }
                    else {
                        e.parse.append('<div class="not-found" style="padding-top:50px"><i class="layui-icon">&#xe664;</i><h5>暂无任何内容</h5></div>');
                    }
                }
            }
        }
    });
});