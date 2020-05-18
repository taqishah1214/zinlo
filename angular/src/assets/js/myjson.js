
function initeditor()
{
  alert();
var quill = new Quill('#editor', {

    theme: 'snow',
    modules: {
      imagePaste: {},
      toolbar: '#toolbar'
    }
  });
 
 return quill;
}

