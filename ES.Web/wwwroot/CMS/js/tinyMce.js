// Define TinyMCE settings globally
var tinyMCEInitSettings = {
    selector: 'textarea[id^="tinyMCE"]',
    plugins: 'anchor autolink charmap codesample emoticons link lists searchreplace table visualblocks accordion wordcount code',
    toolbar: [
        'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link table | spellcheckdialog a11ycheck typography | align lineheight',
        'checklist numlist bullist indent outdent | emoticons charmap | removeformat forecolor | backcolor wordcount code'
    ],
    tinycomments_mode: "embedded",
    extended_valid_elements: [
        'script[src|async|defer|type|charset]',
        'span[*]',
        'iframe[src|width|height|name|align|style|class|allowfullscreen|frameborder|border|marginwidth|marginheight|scrolling|loading|referrerpolicy]',
        'div[*]'
    ].join(','),

    valid_children: '+h4[span],+div[iframe],+section[iframe],+article[iframe],+main[iframe]',
    verify_html: false,
    sandbox_iframes: false,
    relative_urls: false,
    noneditable_class: 'nonedit',
    editable_class: 'editable',
    allow_unsafe_link_target: true, 
    relative_urls: false,
    remove_script_host: false,
    convert_urls: false, 
    media_live_embeds: true,
    media_alt_source: false,
    media_poster: false,
    content_style: 'body { font-family: Arial, sans-serif; }',
    setup: (editor) => {
        editor.on('BeforeSetContent', (e) => {
            // Ensure content is treated as raw, preventing unwanted modifications
            e.content = e.content;
        });
        editor.on('GetContent', (e) => {
            // Return content as-is without additional processing
            e.content = e.content;
        });
    }
};
// Initialize TinyMCE globally
tinymce.init(tinyMCEInitSettings);


function upload(form) {
    tinymce.activeEditor.uploadImages(function (success) {
        form.submit();
    });
    return false;
}

// Prevent Bootstrap dialog from blocking focusin
document.addEventListener('focusin', (e) => {
    if (e.target.closest(".tox-tinymce-aux, .moxman-window, .tam-assetmanager-root") !== null) {
        e.stopImmediatePropagation();
    }
});