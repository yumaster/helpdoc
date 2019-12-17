layui.use(['layer', 'element'], function () {
    var layer = layui.layer,
        element = layui.element,
        hltree = {},
        hlUE = {},
        e = {
            add: $('.hl-add'),
            revise: $('.hl-revise'),
            del: $('.hl-del'),
            nav: $('.hl-nav'),
            save: $('.hl-save'),
            edit: $('.hl-edit'),
            parse: $('.hl-parse')
        },
        f = {
            req: function (options) {
                $.ajax({
                    url: options.url,
                    data: options.data,
                    type: options.type || 'POST',
                    dataType: 'JSON', cache: false,
                    beforeSend: function () { layer.load(2); },
                    success: function (res) {
                        if (res.status == 0) {
                            if (options.success) typeof options.success == 'function' && options.success(res.result);
                            else layer.msg(res.result);
                        }
                        else if (res.status == 1) {
                            if (options.fail) typeof options.fail == 'function' && options.fail(res);
                            else layer.msg(res.result, { icon: 2 });
                        }
                        else if (res.status == 4) {
                            layer.msg('没有操作权限');
                        }
                        else {
                            layer.msg('请求响应错误', { icon: 2 });
                        }
                    },
                    complete: function () { layer.closeAll('loading'); },
                    error: function (err) {
                        layer.msg('请求异常');
                        console.log(err);
                    }
                });
            },
            //处理httpResult返回的json
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
            },
        };
    $.fn.zTree.init($("#hltree"), {
        data: { key: { isParent: 'i', name: 'n' } },
        async: {
            enable: true,
            type: 'get',
            url: '/admin/gettree',
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

                    hlUE.setHide();
                    e.parse.empty().show();
                    e.save.hide();
                    e.edit.show();
                    if (tn.u != null) {
                        //e.parse.load(tn.u, function () {
                        //    uParse('.hl-parse', {
                        //        rootPath: '/scripts/ueditor/'
                        //    });
                        //});
                        e.parse.append(tn.u);
                    }
                    else {
                        e.parse.append('<div class="not-found" style="padding-top:50px"><i class="layui-icon">&#xe664;</i><h5>暂无任何内容</h5><p>您可以点击编辑创建内容</p></div>');
                    }

                }
            }
        }
    });
    hltree = $.fn.zTree.getZTreeObj('hltree');
    hlUE = UE.getEditor('container', {
        initialFrameHeight: $('.layui-body').height() - 85,
        isShow: false
    });

    e.add.off().on('click', function () {
        var tree = hltree.getSelectedNodes(),
            title = '添加目录',
            data = {};
        if (tree.length > 0) {
            title = '添加【' + tree[0].n + '】的子目录';
            data = { pid: tree[0].id };
        }

        layer.prompt({ title: title }, function (v, li, elem) {
            layer.close(li);
            f.req({
                url: '/admin/addcatalog',
                data: $.extend(data, { name: v }),
                success: function (res) {
                    if (tree.length > 0) {
                        var d = tree[0];
                        if (d.zAsync) {//是否已经进行过异步加载
                            hltree.addNodes(d, res, d.open);
                        }
                        else {
                            hltree.expandNode(d, true, true, true);
                        }
                    }
                    else {
                        hltree.addNodes(null, res);
                    }
                    layer.msg('添加成功');
                }
            });
        });
    });
    e.revise.off().on('click', function () {
        var node = hltree.getSelectedNodes();
        if (node.length > 0) {
            var d = node[0];
            layer.prompt({ title: '编辑:' + d.n, value: d.n }, function (v, li) {
                layer.close(li);
                if (v.length <= 200) {
                    f.req({
                        url: '/admin/rename',
                        data: { id: d.id, name: v },
                        success: function (res) {
                            d.n = v;
                            hltree.updateNode(d);
                            layer.msg(res);
                        }
                    });
                }
                else layer.msg('名称限制200字');
            });
        }
        else layer.msg('请选择目录节点');
    });
    e.del.off().on('click', function () {
        var node = hltree.getSelectedNodes();
        if (node.length > 0) {
            var d = node[0];
            layer.confirm('删除【' + d.n + '】？', { icon: 3, title: '询问' }, function (li) {
                layer.close(li);
                f.req({
                    url: '/admin/delcatalog/' + d.id,
                    success: function (res) {
                        hltree.removeNode(d);
                        layer.msg(res);
                    }
                });
            });
        }
        else layer.msg('请选择目录节点');
    });
    e.save.off().on('click', function () {
        if (hlUE.hasContents()) {
            var node = hltree.getSelectedNodes();
            if (node.length > 0) {
                var d = node[0];
                f.req({
                    url: '/admin/ueditorform',
                    data: { id: d.id, content: hlUE.getContent() },
                    success: function (res) {
                        hlUE.setHide();
                        e.parse.empty().show();
                        e.save.hide();
                        e.edit.show();
                        //e.parse.load(res, function () {
                        //    uParse('.hl-parse', {
                        //        rootPath: '/scripts/ueditor/'
                        //    });
                        //});
                        e.parse.append(res);
                        d.u = res;
                        hltree.updateNode(d);
                        layer.msg('保存成功');
                    }
                });
            }
            else layer.msg('请选择目录节点');
        }
        else {
            layer.msg('没有编辑任何内容');
        }
    });
    e.edit.off().on('click', function () {
        var node = hltree.getSelectedNodes();
        if (node.length > 0) {
            var d = node[0];
            e.parse.empty().hide();
            e.save.show();
            e.edit.hide();
            hlUE.execCommand('cleardoc');
            if (d.u != null) {
                //$.get(d.u, function (ud) {
                //    hlUE.setContent(ud);
                //}, 'html');
                //e.parse.append(d.u);
                hlUE.setContent(d.u);
            }
            hlUE.setShow();
        }
        else layer.msg('请选择目录节点');
    });
});