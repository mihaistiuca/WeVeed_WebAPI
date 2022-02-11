

// $Classes/Enums/Interfaces(filter)[template][separator]
// filter (optional): Matches the name or full name of the current item. * = match any, wrap in [] to match attributes or prefix with : to match interfaces or base classes.
// template: The template to repeat for each matched item
// separator (optional): A separator template that is placed between all templates e.g. $Properties[public $name: $Type][, ]

// More info: http://frhagn.github.io/Typewriter/


export class ProducerViewDto {
    
    public id: string = null;
    public producerName: string = null;
    public producerDescription: string = null;
    public numberOfSeries: number = 0;
    public numberOfSeriesEpisodes: number = 0;
    public numberOfSeriesFollowers: number = 0;
    public profileImageUrl: string = null;
    public emailContact: string = null;
    public facebookContactUrl: string = null;
    public instaContactUrl: string = null;
}
