

// $Classes/Enums/Interfaces(filter)[template][separator]
// filter (optional): Matches the name or full name of the current item. * = match any, wrap in [] to match attributes or prefix with : to match interfaces or base classes.
// template: The template to repeat for each matched item
// separator (optional): A separator template that is placed between all templates e.g. $Properties[public $name: $Type][, ]

// More info: http://frhagn.github.io/Typewriter/


export class VideoCreateInput {
    
    public title: string = null;
    public description: string = null;
    public season: number = null;
    public episode: number = null;
    public videoUrl: string = null;
    public thumbnailUrl: string = null;
    public seriesId: string = null;
    public seriesCategory: string = null;
    public length: number = 0;
    public controlbarThumbnailsUrl: string = null;
}
