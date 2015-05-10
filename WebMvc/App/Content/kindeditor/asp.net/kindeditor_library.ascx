<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="kindeditor_library.ascx.cs" Inherits="NFinal.Content.kindeditor.asp.net.kindeditor_library" %>
<link rel="stylesheet" href="/NFinal/Content/kindeditor/themes/default/default.css" />
<link rel="stylesheet" href="/NFinal/Content/kindeditor/plugins/code/prettify.css" />
<script charset="utf-8" src="/NFinal/Content/kindeditor/kindeditor.js"></script>
<script charset="utf-8" src="/NFinal/Content/kindeditor/lang/zh_CN.js"></script>
<script charset="utf-8" src="/NFinal/Content/kindeditor/plugins/code/prettify.js"></script>
<script>
    KindEditor.ready(function (K) {
        K.create("textarea[data-type='kindeditor']", {
            cssPath: '/NFinal/Content/kindeditor/plugins/code/prettify.css',
            uploadJson: '/NFinal/Content/kindeditor/asp.net/upload_json.ashx',
            fileManagerJson: '/NFinal/Content/kindeditor/asp.net/file_manager_json.ashx',
		    allowFileManager: true
		});
	    prettyPrint();
	    K("input[data-type='upload-img']").each(function () {
	        var id = K(this).attr('id');
	        var img = K(this).attr('data-img');
	        var input = K(this).attr('data-input');
	        var uploadbutton = K.uploadbutton({
	            button: K("#" + id),
	            fieldName: 'imgFile',
	            url: '/NFinal/Content/kindeditor/asp.net/upload_json.ashx',
	            afterUpload: function (data) {
	                if (data.error === 0) {
	                    //alert(data.url);
	                    if (img != "") {
	                        K('#' + img).attr('src', data.url);
	                    }
	                    if (input != "") {
	                        K('#' + input).val(data.url);
	                    }
	                } else {
	                    alert(data.message);
	                }
	            }
	        });
	        uploadbutton.fileBox.change(function (e) {
	            uploadbutton.submit();
	        });
	    });
	});
</script>