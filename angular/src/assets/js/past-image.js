!function(e,t){"object"==typeof exports&&"object"==typeof module?module.exports=t():"function"==typeof define&&define.amd?define([],t):"object"==typeof exports?exports.ImagePaste=t():e.ImagePaste=t()}(this,function(){return function(e){function t(i){if(n[i])return n[i].exports;var o=n[i]={i:i,l:!1,exports:{}};return e[i].call(o.exports,o,o.exports,t),o.l=!0,o.exports}var n={};return t.m=e,t.c=n,t.i=function(e){return e},t.d=function(e,n,i){t.o(e,n)||Object.defineProperty(e,n,{configurable:!1,enumerable:!0,get:i})},t.n=function(e){var n=e&&e.__esModule?function(){return e.default}:function(){return e};return t.d(n,"a",n),n},t.o=function(e,t){return Object.prototype.hasOwnProperty.call(e,t)},t.p="",t(t.s=0)}([function(e,t,n){"use strict";function i(e,t){if(!(e instanceof t))throw new TypeError("Cannot call a class as a function")}Object.defineProperty(t,"__esModule",{value:!0}),n.d(t,"ImagePaste",function(){return o});var o=function e(t){var n=this,o=arguments.length>1&&void 0!==arguments[1]?arguments[1]:{};i(this,e),this.handlePaste=function(e){var t=e.clipboardData,i=void 0,o=void 0,r=void 0;if(t&&(i=t.items)){o=i[0],r=t.types||[];for(var a=0;a<r.length;a++)if("Files"===r[a]){o=i[a];break}if(o&&"file"===o.kind&&o.type.match(/^image\//i)){e.preventDefault();var s=o.getAsFile(),u=n.config.addImageBlob;u&&"[object Function]"==={}.toString.call(u)?u(s,n.insertImg):n.toBase64(s)}}},this.toBase64=function(e){var t=new FileReader;t.onload=function(e){n.insertImg(e.target.result)},t.readAsDataURL(e)},this.insertImg=function(e){var t=(n.quill.getSelection()||{}).index||n.quill.getLength()-1;n.quill.insertEmbed(t,"image",e,"user"),setTimeout(function(){n.quill.setSelection(t+1)},0)},this.quill=t,this.config=o,t.root.addEventListener("paste",this.handlePaste,!1)};window.Quill&&window.Quill.register("modules/imagePaste",o)}])});