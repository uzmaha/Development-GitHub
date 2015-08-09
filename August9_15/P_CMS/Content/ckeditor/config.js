/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */


// Define changes to default configuration here. For example:
// config.language = 'fr';
// config.uiColor = '#AADC6E';

CKEDITOR.editorConfig = function (config) {
    config.toolbar = 'Full';
    config.removePlugins = 'advance';
    //disable upload tab
    // Remove multiple plugins.
    config.removePlugins = 'elementspath,save,font';
    config.toolbar_Full = [
        { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline'] },
        {name: 'paragraph', items: ['NumberedList', 'BulletedList', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock']},
        { name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
        { name: 'insert', items: ['Image'] },
     //    { name: 'insert', items: ['Image', 'Flash'] },
        { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
    ];
};