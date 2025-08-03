$(function() {
  // Когато потребителят променя поле, маркираме го като "dirty"
  $(document).on('input change', 'form :input', function() {
    $(this).data('changed', true);
    });

  // При submit: всички полета се маркират като променени, за да се покажат грешки
  $(document).on('submit', 'form', function() {
    $(this).find(':input').each(function() {
      $(this).data('changed', true);
        });
    });

  $.validator.setDefaults({
    onfocusout: function(element) {
            if ($(element).data('changed')) {
                this.element(element);
            }
        },
    onkeyup: function(element) {
            if ($(element).data('changed')) {
                this.element(element);
            }
        }
    });
});

