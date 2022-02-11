

// $Classes/Enums/Interfaces(filter)[template][separator]
// filter (optional): Matches the name or full name of the current item. * = match any, wrap in [] to match attributes or prefix with : to match interfaces or base classes.
// template: The template to repeat for each matched item
// separator (optional): A separator template that is placed between all templates e.g. $Properties[public $name: $Type][, ]

// More info: http://frhagn.github.io/Typewriter/


export class VideoDisplayCarouselDto {
    
    public id: string = null;
    public title: string = null;
    public season: number = null;
    public episode: number = null;
    public thumbnailUrl: string = null;
    public length: number = 0;
    public numberOfViews: number = 0;
    public numberOfLikes: number = 0;
    public userProducerId: string = null;
    public seriesId: string = null;
    public seriesName: string = null;
    public seriesThumbnail: string = null;
    public encodedVideoKey: string = null;
    public createdDate: Date = new Date(0);
}
