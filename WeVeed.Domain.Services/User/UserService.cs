using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public class UserService : IUserService
    {
        const string UserTypeText = "user";
        const string ProducerTypeText = "producer";
        const int TopCountRecent = 10;
        const int TopCountTop = 20;
        const int SearchTopCount = 8;

        private IMongoCollection<User> _userCollection;

        public UserService(IMongoDatabase mongoDatabase)
        {
            _userCollection = mongoDatabase.GetCollection<User>("user");
        }

        public async Task<bool> ValidateProducerByAdmin(ValidateProducerByAdminInput input)
        {
            var filter = Builders<User>.Filter.Eq(a => a.UserType, ProducerTypeText) & Builders<User>.Filter.Eq(a => a.Id, new ObjectId(input.ProducerId));
            var update = Builders<User>.Update.Set(a => a.IsProducerValidatedByAdmin, true);

            var updateResult = await _userCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged;
        }

        public async Task<List<User>> SearchProducerAsync(string word)
        {
            var regWord = "/.*" + word + ".*/i";
            var filter = Builders<User>.Filter.Eq(a => a.UserType, ProducerTypeText)
                & Builders<User>.Filter.Regex(a => a.ProducerName, new BsonRegularExpression(regWord))
                & Builders<User>.Filter.Where(a => a.IsActive && a.IsProducerValidatedByAdmin);
            var sort = Builders<User>.Sort.Descending(a => a.NumberOfSeriesFollowers);
            var options = new FindOptions<User>
            {
                Sort = sort,
                Skip = 0,
                Limit = SearchTopCount
            };

            var users = (await _userCollection.FindAsync(filter, options)).ToList();
            return users;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _userCollection.Find(a => true).ToListAsync();
            return users;
        }

        // IMPORTANT
        public async Task<List<User>> GetMostPopularProducersAsync()
        {
            var filter = Builders<User>.Filter.Eq(a => a.UserType, ProducerTypeText) & Builders<User>.Filter.Where(a => a.IsActive && a.IsProducerValidatedByAdmin);
            var sort = Builders<User>.Sort.Descending(a => a.NumberOfSeriesFollowers);
            var options = new FindOptions<User>
            {
                Sort = sort,
                Skip = 0,
                Limit = TopCountTop
            };

            var users = (await _userCollection.FindAsync(filter, options)).ToList();
            return users;
        }

        // IMPORTANT
        public async Task<List<User>> GetMostRecentProducersAsync()
        {
            var filter = Builders<User>.Filter.Eq(a => a.UserType, ProducerTypeText) & Builders<User>.Filter.Where(a => a.IsActive && a.IsProducerValidatedByAdmin);
            var sort = Builders<User>.Sort.Descending(a => a.CreatedDate);
            var options = new FindOptions<User>
            {
                Sort = sort,
                Skip = 0,
                Limit = TopCountRecent
            };

            var users = (await _userCollection.FindAsync(filter, options)).ToList();
            return users;
        }

        public async Task<List<User>> GetDiscoverMostRecentProducersAsync()
        {
            var filter = Builders<User>.Filter.Eq(a => a.UserType, ProducerTypeText) & Builders<User>.Filter.Where(a => a.IsActive && a.IsProducerValidatedByAdmin);
            var sort = Builders<User>.Sort.Descending(a => a.CreatedDate);
            var options = new FindOptions<User>
            {
                Sort = sort,
                Skip = 0,
                Limit = 20
            };

            var users = (await _userCollection.FindAsync(filter, options)).ToList();
            return users;
        }

        public User GetById(string id)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(id));
            var user = _userCollection.Find(filter).FirstOrDefault();
            return user;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(id));
            var user = await _userCollection.Find(filter).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Email, email);
            var user = await _userCollection.Find(filter).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetByResetTokenAsync(string resetToken)
        {
            var filter = Builders<User>.Filter.Eq(a => a.ResetToken, resetToken);
            var user = await _userCollection.Find(filter).FirstOrDefaultAsync();
            return user;
        }

        public async Task<List<User>> GetAllByIdsList(List<string> idsList, bool ignoreSeriesWithProducerNotValidated = false)
        {
            var idsBsonArray = idsList.Select(a => new ObjectId(a));
            if (ignoreSeriesWithProducerNotValidated)
            {
                var filter = Builders<User>.Filter.In(a => a.Id, idsBsonArray.ToArray()) & Builders<User>.Filter.Where(a => a.IsProducerValidatedByAdmin);
                var users = (await _userCollection.FindAsync(filter)).ToList();
                return users;
            }
            else
            {
                var filter = Builders<User>.Filter.In(a => a.Id, idsBsonArray.ToArray());
                var users = (await _userCollection.FindAsync(filter)).ToList();
                return users;
            }
        }

        public async Task<User> GetByFacebookIdAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(a => a.FacebookUserId, id);
            var user = await _userCollection.Find(filter).FirstOrDefaultAsync();
            return user;
        }

        public async Task<bool> ResetPasswordAsync(string userId, string newPassword)
        {
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(userId));
            var update = Builders<User>.Update.Set(a => a.PasswordHash, passwordHash)
                                                .Set(a => a.PasswordSalt, passwordSalt)
                                                .Set(a => a.ResetDate, default(DateTime))
                                                .Set(a => a.IsResetTokenActive, false)
                                                .Set(a => a.ResetToken, null)
                                                .Set(a => a.IsActive, true)
                                                .Set(a => a.ActivationDate, DateTime.Now);

            var updateResult = await _userCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged;
        }

        public async Task<string> CreateAsync(UserRegisterInput input, Guid guid)
        {
            CreatePasswordHash(input.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var producerName = input.UserType == "producer" ? input.ProducerName.Trim() : null;

            var dbUser = new User
            {
                IsActive = false,
                Email = input.Email.Trim().ToLower(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                ActivationCode = guid.ToString(),
                ActivationDate = DateTime.MinValue,
                FirstName = input.FirstName.Trim(),
                LastName = input.LastName.Trim(),
                ProducerName = producerName,
                UserType = input.UserType,
                Role = "user",
                FacebookUserId = null,
                IsProducerValidatedByAdmin = false
            };

            await _userCollection.InsertOneAsync(dbUser);
            
            return dbUser.Id.ToString();
        }

        public async Task<string> CreateWithFBAsync(UserFBRegisterInput input)
        {
            var producerName = input.UserType == "producer" ? input.ProducerName.Trim() : null;

            var dbUser = new User
            {
                IsActive = true,
                Email = input.Email?.Trim()?.ToLower(),
                PasswordHash = null,
                PasswordSalt = null,
                ActivationCode = null,
                ActivationDate = DateTime.Now,
                FirstName = input.FirstName?.Trim(),
                LastName = input.LastName?.Trim(),
                ProducerName = producerName,
                UserType = input.UserType,
                Role = "user",
                FacebookUserId = input.FacebookUserId,
                ProfileImageUrl = input.ThumbnailUrl
            };

            await _userCollection.InsertOneAsync(dbUser);

            return dbUser.Id.ToString();
        }

        public async Task<bool> SetResetPasswordInformationAsync(string userId, string resetToken)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(userId));
            var update = Builders<User>.Update.Set(a => a.ResetToken, resetToken)
                                                .Set(a => a.IsResetTokenActive, true)
                                                .Set(a => a.ResetDate, DateTime.Now);

            var updateResult = await _userCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged;
        }

        public async Task<bool> UpdateUserAsync(string id, UserUpdateInfoInput input)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(id));
            var update = Builders<User>.Update.Set(a => a.FirstName, input.FirstName)
                                                .Set(a => a.LastName, input.LastName);
            if (input.HasProfileImageChanged)
            {
                update = update.Set(a => a.ProfileImageUrl, input.ProfileImageUrl);
            }

            var updateResult = await _userCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged;
        }

        public async Task<bool> UpdateProducerAsync(string id, ProducerUpdateInfoInput input)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(id));
            var update = Builders<User>.Update.Set(a => a.FirstName, input.FirstName)
                                                .Set(a => a.LastName, input.LastName)
                                                .Set(a => a.ProducerName, input.ProducerName)
                                                .Set(a => a.ProducerDescription, input.ProducerDescription)
                                                .Set(a => a.EmailContact, input.EmailContact)
                                                .Set(a => a.FacebookContactUrl, input.FacebookContactUrl)
                                                .Set(a => a.InstaContactUrl, input.InstaContactUrl);
            if (input.HasProfileImageChanged)
            {
                update = update.Set(a => a.ProfileImageUrl, input.ProfileImageUrl);
            }

            var updateResult = await _userCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged;
        }

        public async Task<bool> BecomeProducerAsync(string id, UserBecomeProducerInput input)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(id));
            var update = Builders<User>.Update.Set(a => a.ProducerName, input.ProducerName)
                                                .Set(a => a.ProducerDescription, input.ProducerDescription)
                                                .Set(a => a.UserType, ProducerTypeText);

            var updateResult = await _userCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(id));
            var deleteResult = await _userCollection.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged;
        }

        public async Task<bool> ConfirmAccountAsync(string guid)
        {
            var user = await _userCollection.Find(a => a.ActivationCode == guid).FirstOrDefaultAsync();

            if(user == null)
            {
                return false;
            }

            var filter = Builders<User>.Filter.Eq(a => a.Id, user.Id);
            var update = Builders<User>.Update.Set(a => a.ActivationCode, null)
                                                .Set(a => a.ActivationDate, DateTime.Now)
                                                .Set(a => a.IsActive, true);
            var updateResult = await _userCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged;
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            email = email.Trim().ToLower();
            var user = await _userCollection.Find(a => a.Email == email).FirstOrDefaultAsync();

            if(user == null)
            {
                return null;
            }

            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        public async Task<List<string>> GetNotActivatedUserIds()
        {
            var filter = Builders<User>.Filter.Where(a => !a.IsProducerValidatedByAdmin && a.UserType == "producer");
            var users = (await _userCollection.FindAsync(filter)).ToList();
            return users.Select(a => a.Id.ToString()).ToList();
        }

        public async Task<List<string>> GetActivatedUserIds()
        {
            var filter = Builders<User>.Filter.Where(a => a.IsProducerValidatedByAdmin || a.UserType != "producer");
            var users = (await _userCollection.FindAsync(filter)).ToList();
            return users.Select(a => a.Id.ToString()).ToList();
        }

        #region User/Producer Followed Series

        public async Task<bool> AddSeriesInUserFollowedSeries(string userId, SeriesFollowInput input)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (user.SeriesFollowed.Contains(input.SeriesId))
            {
                return false;
            }

            var currentSeriesList = user.SeriesFollowed;
            currentSeriesList.Add(input.SeriesId);

            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(userId));
            var update = Builders<User>.Update.Set(a => a.SeriesFollowed, currentSeriesList);
            var updateResult = await _userCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged;
        }

        public async Task<bool> RemoveSeriesFromUserFollowedSeries(string userId, SeriesFollowInput input)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (!user.SeriesFollowed.Contains(input.SeriesId))
            {
                return false;
            }

            var currentSeriesList = user.SeriesFollowed;
            currentSeriesList.Remove(input.SeriesId);

            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(userId));
            var update = Builders<User>.Update.Set(a => a.SeriesFollowed, currentSeriesList);
            var updateResult = await _userCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged;
        }

        public async Task<bool> IncrementProducerSeriesFollowersCount(string producerId)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(producerId));
            var incremenetUpdate = Builders<User>.Update.Inc(a => a.NumberOfSeriesFollowers, 1);
            var updateResult = await _userCollection.UpdateOneAsync(filter, incremenetUpdate);
            return updateResult.IsAcknowledged;
        }

        public async Task<bool> DecrementProducerSeriesFollowersCount(string producerId)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(producerId));
            var incremenetUpdate = Builders<User>.Update.Inc(a => a.NumberOfSeriesFollowers, -1);
            var updateResult = await _userCollection.UpdateOneAsync(filter, incremenetUpdate);
            return updateResult.IsAcknowledged;
        }

        public async Task<bool> IncrementProducerViewsCount(string producerId)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(producerId));
            var incremenetUpdate = Builders<User>.Update.Inc(a => a.NumberOfViewsOnVideos, 1);
            var updateResult = await _userCollection.UpdateOneAsync(filter, incremenetUpdate);
            return updateResult.IsAcknowledged;
        }

        public async Task<List<User>> GetMostFollowedProducers()
        {
            var filter = Builders<User>.Filter.Eq(a => a.UserType, ProducerTypeText) & Builders<User>.Filter.Where(a => a.IsActive && a.IsProducerValidatedByAdmin);
            var sort = Builders<User>.Sort.Descending(a => a.NumberOfSeriesFollowers);
            var options = new FindOptions<User>
            {
                Sort = sort,
                Skip = 0,
                Limit = 50
            };

            var users = (await _userCollection.FindAsync(filter, options)).ToList();
            return users;
        }

        public async Task<List<User>> GetMostViewedProducers()
        {
            var filter = Builders<User>.Filter.Eq(a => a.UserType, ProducerTypeText) & Builders<User>.Filter.Where(a => a.IsActive && a.IsProducerValidatedByAdmin);
            var sort = Builders<User>.Sort.Descending(a => a.NumberOfViewsOnVideos);
            var options = new FindOptions<User>
            {
                Sort = sort,
                Skip = 0,
                Limit = 50
            };

            var users = (await _userCollection.FindAsync(filter, options)).ToList();
            return users;
        }

        #endregion

        #region Unicity Validators

        public async Task<bool> IsProducerNameUnique(string producerName, string id)
        {
            producerName = producerName.Trim().ToLower();
            var regWord = "/^(\\s*)" + producerName + "(\\s*)$/i";

            if (string.IsNullOrEmpty(id))
            {
                var filter = Builders<User>.Filter.Regex(a => a.ProducerName, new BsonRegularExpression(regWord));
                var count = await _userCollection.Find(filter).CountDocumentsAsync();
                return count == 0;
            }
            else
            {
                var filter = Builders<User>.Filter.Ne(a => a.Id, new ObjectId(id)) & Builders<User>.Filter.Regex(a => a.ProducerName, new BsonRegularExpression(regWord));
                var count = await _userCollection.Find(filter).CountDocumentsAsync();
                return count == 0;
            }
        }

        public async Task<bool> IsEmailUnique(string email, string id)
        {
            email = email.Trim().ToLower();
            var regWord = "/^(\\s*)" + email + "(\\s*)$/i";

            if (string.IsNullOrEmpty(id))
            {
                var filter = Builders<User>.Filter.Regex(a => a.Email, new BsonRegularExpression(regWord));
                var count = await _userCollection.Find(filter).CountDocumentsAsync();
                return count == 0;
            }
            else
            {
                var filter = Builders<User>.Filter.Ne(a => a.Id, new ObjectId(id)) & Builders<User>.Filter.Regex(a => a.Email, new BsonRegularExpression(regWord));
                var count = await _userCollection.Find(filter).CountDocumentsAsync();
                return count == 0;
            }
        }

        #endregion

        #region Utils

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        #endregion
    }
}
