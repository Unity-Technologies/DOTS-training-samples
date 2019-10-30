var slideNumber = 1;
advanceSlide(0);
document.getElementById('slides').addEventListener('click', changeSlide);

function changeSlide(evt){
    var rect = evt.target.getBoundingClientRect();

    if(evt.clientX > rect.left + (rect.right - rect.left)/2){ 
        advanceSlide(1);
    } else {
        advanceSlide(-1);
    }
}

function advanceSlide(n) {
    slideNumber += n;
    var slides = document.getElementsByClassName("infographic");
    if (slideNumber > slides.length) 
    {
        slideNumber = 1;
    } 
    else if (slideNumber < 1) 
    {
        slideNumber = slides.length;
    }
    
    for (var i = 0; i < slides.length; i++) 
    {
        slides[i].style.display = "none"; 
    }
    slides[slideNumber - 1].style.display = "block"; 
}